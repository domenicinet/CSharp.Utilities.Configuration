using System;
using System.Collections.Generic;
using System.Xml;

namespace Domenici.Utilities.Configuration
{
    public class SettingsLoader
    {
        private List<SettingProperties> settingsList;
        private string sectionSeparator;
        private bool ignoreDuplicates = false;
        private bool overrideExisting = false;
        private bool ignoreExternalSource = false;

        public SettingsLoader(List<SettingProperties> settingsList, string sectionSeparator, bool ignoreDuplicates = false, bool overrideExisting = false)
        {
            this.settingsList = settingsList;
            this.sectionSeparator = sectionSeparator;
            this.ignoreDuplicates = ignoreDuplicates;
            this.overrideExisting = overrideExisting;
        }

        public XmlDocument LoadSettings(string filePath, bool ignoreExternalSource = false)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            return this.LoadSettings(doc, ignoreExternalSource);
        }

        public XmlDocument LoadSettings(XmlDocument doc, bool ignoreExternalSource = false)
        {
            this.ignoreExternalSource = ignoreExternalSource;

            #region Load settings file
            // Get all settings at root level
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("item"))
            {
                if (!this.ignoreExternalSource || (null == node.Attributes["source"] || "external" != node.Attributes["source"].Value))
                {
                    try
                    {
                        string key = node.Attributes["key"].Value;

                        if (this.overrideExisting)
                        {
                            settingsList.RemoveAll(x => x.Key.Contains(key));
                        }

                        string value = null;

                        if (null != node.Attributes["value"])
                        {
                            value = node.Attributes["value"].Value;
                        }
                        else
                        {
                            // This value appears to be a multilined text within a CDATA block.
                            value = node.FirstChild.Value;
                        }

                        settingsList.Add(new SettingProperties()
                        {
                            Key = key,
                            Value = value,
                            Type = node.Attributes["type"] == null ? "string" : (string.IsNullOrEmpty(node.Attributes["type"].Value) ? "string" : node.Attributes["type"].Value)
                        });

                        Console.WriteLine($"Found settings: {key}");
                    }
                    catch (Exception e)
                    {
                        if (!this.ignoreDuplicates) throw e;
                    }
                }
            }

            // Get inner sections
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("section"))
            {
                LoadSection(null, node);
            }

            return doc;
            #endregion
        }

        private void LoadSection(string path, XmlNode sectionNode)
        {
            Console.WriteLine($"Found section: {sectionNode.Attributes["name"].Value}");

            // Get all settings
            foreach (XmlNode node in sectionNode.SelectNodes("item"))
            {
                try
                {
                    string key   = node.Attributes["key"].Value;
                    
                    if (!this.ignoreExternalSource || (null == node.Attributes["source"] || "external" != node.Attributes["source"].Value))
                    {
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            key = string.Format("{0}{1}{2}{3}{4}",
                                          path,
                                          this.sectionSeparator,
                                          sectionNode.Attributes["name"].Value,
                                          this.sectionSeparator,
                                          key);
                        }
                        else
                        {
                            key = string.Format("{0}{1}{2}",
                                          sectionNode.Attributes["name"].Value,
                                          this.sectionSeparator,
                                          key);
                        }

                        string value = null;

                        if (null != node.Attributes["value"])
                        {
                            value = node.Attributes["value"].Value;
                        }
                        else
                        {
                            // This value is a multi-line string, possibly within a CDATA block.
                            value = node.FirstChild.Value;
                        }

                        this.settingsList.Add(new SettingProperties()
                        {
                            Key   = key,
                            Value = value,
                            Type  = node.Attributes["type"] == null ? "string" : (string.IsNullOrEmpty(node.Attributes["type"].Value) ? "string" : node.Attributes["type"].Value)
                        });

                        Console.WriteLine($"Found setting: {node.Attributes["key"].Value}");
                    }
                }
                catch (Exception e)
                {
                    if (!this.ignoreDuplicates) throw e;
                }
            }

            // Get inner sections
            foreach (XmlNode node in sectionNode.SelectNodes("section"))
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    LoadSection(
                        string.Format("{0}{1}{2}",
                                      path,
                                      SettingsManager.SectionSeparator,
                                      sectionNode.Attributes["name"].Value),
                        node);
                }
                else
                {
                    LoadSection(sectionNode.Attributes["name"].Value, node);
                }
            }
        }
    }
}