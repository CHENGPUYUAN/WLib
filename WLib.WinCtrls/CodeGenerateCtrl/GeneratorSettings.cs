using System.Collections.Generic;

namespace WLib.WinCtrls.CodeGenerateCtrl
{
    internal class GeneratorSettings
    {
        public int DbType { get; internal set; }
        public string ConnectionString { get; internal set; }
        public string CSharpSavePath { get; internal set; }
        public CSharpSettings CSharpSettings { get; internal set; }
        public string JavaSavePath { get; internal set; }
        public JavaSettings JavaSettings { get; internal set; }
    }

    internal class CSharpSettings
    {
        public string NameSpace { get; internal set; }
        public string Inherits { get; internal set; }
        public List<string> Usings { get; internal set; }
        public List<string> ClassAttributes { get; internal set; }
        public List<string> PropertyAttributes { get; internal set; }
    }
    internal class JavaSettings
    {
        public string Package { get; internal set; }
        public string Extends { get; internal set; }
        public string Implements { get; internal set; }
        public List<string> Imports { get; internal set; }
        public List<string> ClassAnnotations { get; internal set; }
        public List<string> PropertyAnnotations { get; internal set; }
    }
}