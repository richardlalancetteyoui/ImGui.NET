using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ImGuiNET
{
    class KeyPrinter : youiBaseListener
    {
        public override void EnterEveryRule(ParserRuleContext context)
        {
            base.EnterEveryRule(context);
        }
    }
}