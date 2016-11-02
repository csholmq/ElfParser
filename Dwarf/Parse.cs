using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfParser.Dwarf
{
    class Parse
    {
        // Parse compilation unit from .debug_info
        public static CompilationUnit CU(List<byte> infoData, ref int index, List<Abbreviation> abbrevList)
        {
            CompilationUnit result;
            if (index >= infoData.Count)
                result = null;
            else
            {
                var cuId = index;
                var cuh = CUH(infoData, ref index, cuId);
                var cuLength = cuh.Length + 4;

                var abbrevListFiltered = abbrevList.Where(a => a.Offset == cuh.AbbrevOffset).ToList();
                //				var abbrevListFiltered = (from a in abbrevList
                //				where (long)a.Offset == (long)((ulong)cuh.AbbrevOffset)
                //				select a).ToList<Abbreviation>();

                var dieList = new List<DebuggingInformationEntry>();
                while (index < cuId + cuLength)
                {
                    var die = DIE(infoData, ref index, abbrevListFiltered, cuId);
                    dieList.Add(die);
                }
                result = new CompilationUnit(cuh, dieList);
            }
            return result;
        }

        // Parse DIE from .debug_info
        static DebuggingInformationEntry DIE(List<byte> infoData, ref int index, List<Abbreviation> abbrevList, int cuId)
        {
            var id = index;
            var code = LEB128.ReadUnsigned(infoData, ref index);
            DebuggingInformationEntry result;
            if (code == 0)
                result = null;
            else
            {
                var abbrev = abbrevList.Find(a => a.Code == code);
                var die = new DebuggingInformationEntry(id, code, abbrev.Tag, abbrev.HasChildren);
                foreach (var abbrevAttr in abbrev.AttributeList)
                {
                    List<byte> value = Read.AttributeValue(infoData, ref index, abbrevAttr, cuId);
                    Attribute attr = new Attribute(abbrevAttr.Name, abbrevAttr.Form, value.ToArray());
                    die.AddAttribute(attr);
                }
                result = die;
            }
            return result;
        }

        // Parse abbreviation from .debug_abbrev
        public static Abbreviation Abbreviation(List<byte> abbrevData, ref int index, int startIndex)
        {
            var code = LEB128.ReadUnsigned(abbrevData, ref index);
            Abbreviation result;
            if (code == 0)
                result = null;
            else
            {
                var tag = LEB128.ReadUnsigned(abbrevData, ref index);
                var hasChildren = abbrevData[index];
                index++;
                var abbreviation = new Abbreviation(startIndex, code, (DW_TAG)tag, (DW_CHILDREN)hasChildren);

                while (index < abbrevData.Count)
                {
                    var name = LEB128.ReadUnsigned(abbrevData, ref index);
                    var form = LEB128.ReadUnsigned(abbrevData, ref index);
                    if (name == 0 && form == 0)
                        break;
                    abbreviation.AddAttribute(new Attribute(name, form));
                }
                result = abbreviation;
            }
            return result;
        }

        // Parse compilation unit header from .debug_info
        static CompilationUnitHeader CUH(List<byte> infoData, ref int index, int id)
        {
            var cuhLength = 11;
            var cuhData = infoData.GetRange(index, cuhLength).ToArray();
            index += cuhLength;

            var length = BitConverter.ToUInt32(cuhData, 0);
            var version = BitConverter.ToUInt16(cuhData, 4);
            var offset = BitConverter.ToUInt32(cuhData, 6);
            var size = cuhData[10];

            return new CompilationUnitHeader(id, length, version, offset, size);
        }

        
    }
}
