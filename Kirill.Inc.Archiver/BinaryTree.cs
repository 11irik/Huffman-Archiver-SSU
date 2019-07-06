using System;
using System.Collections.Generic;

namespace Example
{
    public class Node : IComparable<Node>
    {
        public char Symbol; 
        public int Counter;
        public Node Left; 
        public Node Rigth; 

        public Node(char b, int c)
        {
            Symbol = b;
            Counter = c;
            Left = null;
            Rigth = null;
        }

        public Node(Node a, Node b)
        {
            Counter = a.Counter + b.Counter;
            Left = a;
            Rigth = b;
        }

        public List<bool> InOrder(char symbol, List<bool> data)
        {
            if (Rigth == null && Left == null)
            {
                if (this.Symbol == symbol)
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;
 
                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);
 
                    left = Left.InOrder(symbol, leftPath);
                }
 
                if (Rigth != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Rigth.InOrder(symbol, rightPath);
                }
 
                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }

        public int CompareTo(Node node)
        {
            if (this.Counter == node.Counter)
            {
                return 0;
            }
            else
            {
                if (this.Counter > node.Counter)
                {
                    return 1;
                }

                else
                {
                    return -1;
                }
            }
        }
    }
}


        
        
       



