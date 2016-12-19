﻿using System.Collections.Generic;

namespace AParser
{
    public class OracleToFloatTranslator: ASTNodeTranslator
    {
        public override void Translate(IATranslator translator, ASTNodeList newNodes, ASTNodeList subNodes, IASTNodeFactory nodeFactory)
        {
            // TO_FLOAT('123') => cast(decode('123', ' ', 0.00, '123') as number(30,8) )
            Translator = translator;
            NodeFactory = nodeFactory;

            IASTNode newNode = NodeFactory.CreateNode("cast", false);
            newNode.SubNodes.Add(NodeFactory.CreateNode(ASTStartParenthesesNode.KeyWord, false));
            newNode.SubNodes.Add(NodeFactory.CreateFunctionParameterNode());
            newNode.SubNodes.Add(NodeFactory.CreateNode(ASTEndParenthesesNode.KeyWord));

            IASTNode parameter = CreateFunctionParameterNodeWithSubNodes(subNodes[1]);
            AddDecodeFunction(newNode.SubNodes[1].SubNodes, parameter);

            newNodes.Add(newNode);
        }

        private void AddDecodeFunction(ASTNodeList newNodes, IASTNode parameter)
        {
            List<IASTNode> parameters = new List<IASTNode>
            {
                parameter,
                CreateFunctionParameterNode("' '"),
                CreateFunctionParameterNode("0.00"),
                parameter
            };

            AddFunction(newNodes, "decode", parameters);
            newNodes.Add(NodeFactory.CreateNode("as"));
            AddFunction(newNodes, 
                "number", 
                CreateFunctionParameterNode("30"),
                CreateFunctionParameterNode("8"));
        }
    }
}
