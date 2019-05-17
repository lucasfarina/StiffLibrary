using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StiffLibrary.XML
{
    public static class XMLManager
    {
        public static string GetXMLFile(string path, out XMLFile xml)
        {
            string success = ""; //Empty equals success, error contains error message
            xml = new XMLFile(new XMLPrologue(), new XMLTag("Error", "Couldn't get XMLFile at GetXMLFile(" + path + ").", new XMLTag[0], new XMLAttribute[0]));
            String[] lines = IOManager.GetFile(path);
            String stream = string.Join(string.Empty, lines);
            XMLPrologue prologue;
            string StreamLeft;
            if(GetXMLProlog(stream, out prologue, out StreamLeft))
            {
                xml.Prologue = prologue;
            }
            else
            {
                StreamLeft = stream;
            }
            XMLTag[] tags;
            success = GetTags(StreamLeft, out tags);
            if (success == "")
            {
                if(tags.Length == 1)
                {
                    xml.Root = tags[0];
                }
                else
                {
                    success = "Error! XML should have one, and just one root tag!";
                }
            }
            return success;
        }

        public static void SaveXMLFile(string path, XMLFile xml)
        {
            List<string> lines = new List<string>();
            if(xml.Prologue != null)
            {
                lines.Add(xml.Prologue.ToXmlPrologueText());
            }
            lines.AddRange(xml.Root.ToXmlText());
            IOManager.WriteFile(path, lines.ToArray());
        }

        private static string GetTags(String stream, out XMLTag[] tags)
        {
            string success = "";
            List<XMLTag> tagList = new List<XMLTag>();
            Int32 index = 0;
            Pile<XMLTag> tagsOpened = new Pile<XMLTag>();
            Pile<string> contentsOpened = new Pile<string>();
            string buffer = "";
            int bufferState = 0;//0 = nothing, 1 = tag, 2 = content, 3 = getting End
            //Get inner content of prologueTag
            for (index = 0; index < stream.Length; index++)
            {
                if (bufferState == 0 && stream[index] == '<')//wait for tag
                {
                    bufferState = 1; //read tag
                }
                else if (bufferState == 1)//while reading tag
                {
                    if (stream[index] == '>') //Closed without ending tag
                    {
                        bufferState = 2;
                        XMLTag gatheredTag;
                        if(XMLTagFromContent(buffer,out gatheredTag))
                        {
                            tagsOpened.Push(gatheredTag);
                            buffer = "";
                        }
                        else
                        {
                            success = "Error! BufferState 1, could not gather open tag.";
                            break;
                        }
                    }
                    else if (stream[index] == '/') //Closed ending tag
                    {
                        if(index+1 < stream.Length)
                        {
                            if(stream[index+1] == '>')//Closed and ended correctly
                            {
                                index += 1;
                                XMLTag gatheredTag;
                                if (XMLTagFromContent(buffer, out gatheredTag))
                                {
                                    if (tagsOpened.PileSize() > 0)
                                    {
                                        tagsOpened.GetTop().AddChild(gatheredTag);
                                        buffer = contentsOpened.Pop();
                                        bufferState = 2;
                                    }
                                    else
                                    {
                                        tagList.Add(gatheredTag);
                                        buffer = "";
                                        bufferState = 0;
                                    }
                                }
                                else
                                {
                                    success = "Error! BufferState 1, could not gather closed tag.";
                                    break;
                                }
                            }
                            else
                            {
                                success = "Error! BufferState 1, hash to close but did not close tag.";
                                break;
                            }
                        }
                        else
                        {
                            success = "Error! Stream ended unexpectedly.";
                            break;
                        }
                    }
                    else//gather tag
                    {
                        buffer += stream[index];
                    }
                }
                else if(bufferState == 2)//while reading content
                {
                    if(stream[index] == '<')
                    {
                        if(index+1 < stream.Length)
                        {
                            if(stream[index + 1] == '/')
                            {
                                index += 1;
                                contentsOpened.Push(buffer);
                                buffer = "";
                                bufferState = 3;
                            }
                            else
                            {
                                contentsOpened.Push(buffer);
                                buffer = "";
                                bufferState = 1;
                            }
                        }
                        else
                        {
                            success = "Error! Buffer 2. Stream ended unexpectedly.";
                            break;
                        }
                    }
                    else
                    {
                        buffer += stream[index];
                    }
                }
                else if(bufferState == 3)
                {
                    if(stream[index] == '>')
                    {
                        XMLTag lastOpened = tagsOpened.Pop();
                        if(buffer == lastOpened.Identifier)
                        {
                            string content = contentsOpened.Pop();
                            lastOpened.Value = content;
                            if(tagsOpened.PileSize() > 0)
                            {
                                tagsOpened.GetTop().AddChild(lastOpened);
                                buffer = contentsOpened.Pop();
                                bufferState = 2;
                            }
                            else
                            {
                                tagList.Add(lastOpened);
                                buffer = "";
                                bufferState = 0;
                            }
                            
                        }
                        else
                        {
                            success = "Error! Buffer 3, opening and ending tag mismatch!.";
                            break;
                        }
                    }
                    else
                    {
                        buffer += stream[index];
                    }
                    
                }
            }
            tags = tagList.ToArray();
            return success;
        }

        private static bool GetXMLProlog(String stream, out XMLPrologue prolog, out String StreamLeft)
        {
            bool success = false;
            prolog = new XMLPrologue();
            Int32 index = 0;
            string buffer = "";
            int bufferState = 0;//0 = nothing, 1 = prologue, 2 = successful getting prologue ended
            //Get inner content of prologueTag
            for(index = 0; index < stream.Length; index++){
                if( bufferState == 0 && stream[index] == '<'){
                    if( index + 1 < stream.Length){
                        if (stream[index + 1] == '?'){
                            index += 1;
                            bufferState = 1;
                        }else{
                            break;
                        }
                    }
                    else{
                        break;
                    }
                }
                else if(bufferState == 1)
                {
                    if(stream[index] == '?')
                    {
                        if (index + 1 < stream.Length)
                        {
                            if(stream[index + 1] == '>')
                            {
                                index += 2;
                                bufferState = 2;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    buffer += stream[index];
                }
            }
            //if got prologue
            if(bufferState == 2)
            {
                string content = buffer.Trim(' ');
                string[] atts = content.Split(' ');
                if(atts[0] == "xml")
                {
                    XMLPrologue prologue = new XMLPrologue();
                    success = true;
                    for(int i = 1; i < atts.Length; i++)
                    {
                        if(!string.IsNullOrEmpty(atts[i].Trim(' ')))
                        {
                            string att = atts[i].Trim(' ');
                            string[] keyAndValue = atts[i].Split('=');
                            if (keyAndValue.Length != 2) { success = false; break; }
                            if (string.IsNullOrEmpty(keyAndValue[0]) || string.IsNullOrEmpty(keyAndValue[1])) { success = false; break; }
                            prologue.SetAttribute(new XMLAttribute(keyAndValue[0], keyAndValue[1].Trim('\"')));
                            success = true;
                        }
                    }
                    if (success && prologue.CheckValid())
                    {
                        prolog = prologue;
                    }
                }
            }



            if(index + 1 < stream.Length)
            {
                StreamLeft = stream.Substring(index);
            }
            else
            {
                StreamLeft = "";
            }
            
            return success;
        }

        private static bool XMLTagFromContent(String content, out XMLTag tag)
        {
            XMLTag possibleTag = new XMLTag("Error", "Error! Could not create tag from stream on XMLTagFromContent", new XMLTag[0], new XMLAttribute[0]);
            bool success = false;
            string contentTrimmed = content.Trim(' ');
            string[] atts = contentTrimmed.Split(' ');
            string tagName = atts[0].Trim(' ');
            if (!string.IsNullOrEmpty(tagName))
            {
                success = true;
                possibleTag = new XMLTag(tagName, "", new XMLTag[0], new XMLAttribute[0]);
                for (int i = 1; i < atts.Length; i++)
                {
                    if(!string.IsNullOrEmpty(atts[i].Trim(' ')))
                    {
                        string att = atts[i].Trim(' ');
                        string[] keyAndValue = atts[i].Split('=');
                        if (keyAndValue.Length != 2) { success = false; break; }
                        if (string.IsNullOrEmpty(keyAndValue[0]) || string.IsNullOrEmpty(keyAndValue[1])) { success = false; break; }
                        possibleTag.SetAttribute(new XMLAttribute(keyAndValue[0], keyAndValue[1].Trim('"')));
                        success = true;
                    }
                }
                if (success && possibleTag.CheckValid())
                {
                    tag = possibleTag;
                }
            }
            tag = possibleTag;
            return success;
        }

        public static void SaveExample(string path)
        {
            XMLPrologue prologue = new XMLPrologue();
            XMLTag root = new XMLTag("Person", string.Empty);
            XMLTag nome = new XMLTag("Name", "João da Silva");
            nome.SetAttribute(new XMLAttribute("Compound", "Yes"));
            root.AddChild(nome);
            root.AddChild(new XMLTag("Age", "15"));
            root.AddChild(new XMLTag("Cidade", "São Paulo"));
            root.SetAttribute(new XMLAttribute("Raca", "Pardo"));
            XMLFile file = new XMLFile(prologue, root);

            XMLManager.SaveXMLFile(path, file);
        }

        public static string ReadSaveExample(string readPath, string writePath)
        {
            string message;
            XMLFile file;
            if ((message = XMLManager.GetXMLFile(readPath, out file)) == "")
            {
                XMLManager.SaveXMLFile(writePath, file);
                return message;
            }
            else
            {
                return message;
            }
        }
    }
}
