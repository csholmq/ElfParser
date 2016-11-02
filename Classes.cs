using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfParser
{
    class Module
    {
        public string Name { get; }
        public List<Variable> VariableList { get; }

        public Module(string name)
        {
            Name = name;
            VariableList = new List<Variable>();
        }

        public void AddVariable(Variable var)
        {
            VariableList.Add(var);
        }
    }

    class Variable
    {
        public string Name { get; }
        public string Type { get; set; }
        public int Address { get; set; }
        public int[] ArraySize { get; set; }
        public int ByteSize { get; set; }
        public List<Variable> VariableList { get; }

        public Variable(string name)
        {
            Name = name;
            Type = "";
            Address = 0;
            ArraySize = new int[2] { 1, 1 };
            ByteSize = 0;
            VariableList = new List<Variable>();
        }
        
        public void AddVariable(Variable var)
        {
            VariableList.Add(var);
        }
    }

    class Typedef
    {
        public string Name { get; }
        public List<Member> MemberList { get; }

        public Typedef(string name)
        {
            Name = name;
            MemberList = new List<Member>();
        }

        public void AddMember(Member member)
        {
            MemberList.Add(member);
        }
    }

    class Member
    {
        public string Name { get; }
        public string TypeName { get; }
        public int RelAddress { get; }

        public Member(string name, string typeName, int relAddress)
        {
            Name = name;
            TypeName = typeName;
            RelAddress = relAddress;
        }
    }
}
