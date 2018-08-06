using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StiffLibrary.XML
{
    [DataContract]
    public class XMLFile
    {
        public XMLFile(XMLTag root)
        {
            Prologue = new XMLPrologue();
        }
        public XMLFile(XMLPrologue prologue, XMLTag root)
        {
            Prologue = prologue;
            Root = root;
        }
        [DataMember]
        public XMLPrologue Prologue;
        [DataMember]
        public XMLTag Root;
    }

    [DataContract]
    public class XMLPrologue : XMLTag
    {
        public XMLPrologue() : base("xml", "")
        {
            this.SetAttribute(new XMLAttribute("version", "1.0"));
            this.SetAttribute(new XMLAttribute("enconding", "UTF-8"));
        }

        public XMLPrologue(string version, string enconding, XMLAttribute[] Attributes) : base("xml", "", new XMLTag[0], Attributes)
        {
            this.SetAttribute(new XMLAttribute("version", version));
            this.SetAttribute(new XMLAttribute("enconding", enconding));
        }

        public new String Value { get { return value; } }
        public override void AddChild(XMLTag childTag)
        {
            //Do nothing
        }
        public override string[] ToXmlText()
        {
            string myOpeningTag = "<?xml";
            string[] allAtt = this.GetAllAttributesNames();
            foreach (string att in allAtt)
            {
                XMLAttribute atri;
                if (this.GetAttribute(att, out atri))
                {
                    myOpeningTag += " " + atri.Identifier + "=\"" + atri.Value + "\"";
                }
            }
            myOpeningTag += "?>";
            List<string> listForm = new List<string>();
            listForm.Add(myOpeningTag);
            return listForm.ToArray();
        }
        public string ToXmlPrologueText()
        {
            string myOpeningTag = "<?xml";
            string[] allAtt = this.GetAllAttributesNames();
            foreach (string att in allAtt)
            {
                XMLAttribute atri;
                if (this.GetAttribute(att, out atri))
                {
                    myOpeningTag += " " + atri.Identifier + "=\"" + atri.Value + "\"";
                }
            }
            myOpeningTag += "?>";
            return myOpeningTag;
        }
    }

    [DataContract]
    public class XMLTag : XMLAttribute
    {

        public XMLTag(string Id, string Value = "") : base(Id, Value)
        {
            this.children = new List<XMLTag>();
            this.attributes = new List<XMLAttribute>();
        }

        public XMLTag(string Id, string Value, XMLTag[] Children, XMLAttribute[] Attributes) : base(Id, Value)
        {
            this.children = new List<XMLTag>();
            this.attributes = new List<XMLAttribute>();
            foreach(XMLTag x in Children)
            {
                AddChild(x);
            }
            foreach (XMLAttribute a in Attributes)
            {
                SetAttribute(a);
            }
        }

        [DataMember]
        private List<XMLTag> children = new List<XMLTag>();
        [DataMember]
        private List<XMLAttribute> attributes = new List<XMLAttribute>();

        public new String Value { get { return value; } set { this.value = value; } } 

        public XMLTag[] GetChild(String name)
        {
            List<XMLTag> tagsList = new List<XMLTag>();
            foreach(XMLTag t in children)
            {
                if(t.Identifier == name)
                {
                    tagsList.Add(t);
                }
            }
            return tagsList.ToArray();
        }

        public bool GetAttribute(String name, out XMLAttribute attribute)
        {
            bool success = false;
            attribute = new XMLAttribute("Error", "Couldn't get Attribute at GetAttribute(" + name + ")");
            foreach(XMLAttribute at in attributes)
            {
                if(at.Identifier == name)
                {
                    attribute = at;
                    success = true;
                }
            }
            return success;
        }

        public void SetAttribute(XMLAttribute attribute) {
            bool contains = false;
            int index = -1;
            for(int i = 0; i < attributes.Count; i++)
            {
                if(attributes[i].Identifier == attribute.Identifier)
                {
                    contains = true;
                    index = i;
                }
            }
            if (contains)
            {
                attributes.RemoveAt(index);
                attributes.Add(attribute);
            }
            else
            {
                attributes.Add(attribute);
            }
        }

        public virtual void AddChild(XMLTag childTag)
        {
            if (childTag.CheckValid())
            {
                children.Add(childTag);
            }
        }

        public string[] GetAllChildrenNames()
        {
            List<string> list = new List<string>();
            foreach(XMLTag child in children)
            {
                list.Add(child.Identifier);
            }
            return list.ToArray();
        }

        public string[] GetAllAttributesNames()
        {
            List<string> list = new List<string>();
            foreach (XMLAttribute child in attributes)
            {
                list.Add(child.Identifier);
            }
            return list.ToArray();
        }

        public bool CheckValid()
        {
            bool success = false;
            success = !string.IsNullOrEmpty(Identifier);
            if (string.IsNullOrEmpty(Value))
            {
                success = children.Count > 0 || attributes.Count > 0;
            }
            return success;
        }

        public virtual string[] ToXmlText()
        {
            List<string> lines = new List<string>();
            string myOpeningTag = "<" + Identifier;
            foreach(XMLAttribute att in attributes)
            {
                myOpeningTag += " " + att.Identifier + "=\"" + att.Value + "\"";
            }
            if(string.IsNullOrEmpty(Value) && children.Count == 0)
            {
                myOpeningTag += "/>";
                lines.Add(myOpeningTag);
            }
            else
            {
                myOpeningTag += ">"+Value;
                if (children.Count > 0)
                {
                    lines.Add(myOpeningTag);
                    foreach (XMLTag child in children)
                    {
                        lines.AddRange(child.ToXmlText());
                    }
                    lines.Add("</" + Identifier + ">");
                }
                else
                {
                    myOpeningTag += "</" + Identifier + ">";
                    lines.Add(myOpeningTag);
                }
                
            }
            return lines.ToArray();
        }
    }

    [DataContract]
    public class XMLAttribute
    {
        public XMLAttribute(String Identifier, String Value = "")
        {
            identifier = Identifier;
            value = Value;
        }

        [DataMember]
        private String identifier;
        [DataMember]
        protected String value;

        public String Identifier { get { return identifier; } }
        public String Value { get { return value; } }


        public bool GetValueAsInt(ref int Value)
        {
            int result;
            bool success = int.TryParse(this.Value, out result);
            Value = result;
            return success;
        }

        public bool GetValueAsFloat(ref float Value)
        {
            float result;
            bool success = float.TryParse(this.Value, out result);
            Value = result;
            return success;
        }

        public bool GetValueAsBool(ref bool Value)
        {
            bool success = false;
            if (String.Equals(this.Value, "1", StringComparison.OrdinalIgnoreCase) || String.Equals(this.Value, "T", StringComparison.OrdinalIgnoreCase) || String.Equals(this.Value, "True", StringComparison.OrdinalIgnoreCase))
            {
                Value = true;
                success = true;
            }
            else if (String.Equals(this.Value, "0", StringComparison.OrdinalIgnoreCase) || String.Equals(this.Value, "F", StringComparison.OrdinalIgnoreCase) || String.Equals(this.Value, "False", StringComparison.OrdinalIgnoreCase))
            {
                Value = false;
                success = true;
            }
            return success;
        }
    }

    public static class Arrays<T>
    {
        public static readonly T[] empty = new T[0];

        //public static readonly T[] Empty { get { return empty; } }
    }
}
