using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MB.Web.Core
{
    public abstract class MBControlBase
    {
        protected MBControlBase(string tagName)
            : this(tagName, TagRenderMode.Normal)
        {
            this.NoContainer = false;
        }

        protected MBControlBase(string tagName, TagRenderMode tagRenderMode)
        {
            this.Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
            this.TagName = tagName;
            this.TagRenderMode = tagRenderMode;
            this.NoContainer = false;
        }

        public void AddClass(string className)
        {
            if (!string.IsNullOrEmpty(className))
            {
                className = className.Trim();
            }

            string currentClassName;

            if (this.Attributes.TryGetValue("class", out currentClassName))
            {
                currentClassName = currentClassName.Trim();

                foreach (string item in className.Split(' '))
                {
                    if ((" " + currentClassName + " ").IndexOf(" " + item + " ") == -1 && !string.IsNullOrEmpty(item))
                    {
                        currentClassName += " " + item;
                    }
                }

                this.Attributes["class"] = currentClassName.Trim();
            }
            else
            {
                this.Attributes["class"] = className;
            }
        }

        public void AddEventScript(string eventKey, string script)
        {
            string newScript = script;

            if (string.IsNullOrEmpty(newScript))
            {
                newScript = newScript.Trim();

                if (!newScript.EndsWith("}")
                    && !newScript.EndsWith(";"))
                {
                    newScript += ";";
                }
            }

            string currentScript;

            if (this.Attributes.TryGetValue(eventKey, out currentScript))
            {
                currentScript = currentScript.Trim();

                if (!currentScript.EndsWith("}")
                    && !currentScript.EndsWith(";"))
                {
                    currentScript += ";";
                }

                this.Attributes[eventKey] = currentScript + " " + newScript;
            }
            else
            {
                this.Attributes[eventKey] = newScript;
            }
        }

        // Outer Container
        private TagBuilder container = null;

        protected TagBuilder GetTagBuilder()
        {
            TagBuilder tagBuilder = new TagBuilder(this.TagName);
            tagBuilder.MergeAttributes(this.HtmlAttributes);

            if (this.Attributes.ContainsKey("class"))
            {
                tagBuilder.AddCssClass(this.Attributes["class"]);
                this.Attributes.Remove("class");
            }

            tagBuilder.MergeAttributes(this.Attributes);
            tagBuilder.InnerHtml = this.InnerHtml;
            return tagBuilder;
        }

        public virtual MvcHtmlString Html(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            string htmlResult = string.Empty;

            if (this.HtmlSource == null)
            {
                StringBuilder htmlCreate = new StringBuilder();

                this.Initialise(viewContext);

                TagBuilder tagBuilder = this.GetTagBuilder();

                using (StringWriter writer = new StringWriter(htmlCreate))
                {
                    writer.Write(tagBuilder.ToString(TagRenderMode));

                    this.RenderCustomHtml(writer, viewContext);
                }

                if (!string.IsNullOrEmpty(this.AppendHtmlTag))
                {
                    htmlCreate.Append(this.AppendHtmlTag);
                }

                htmlResult = htmlCreate.ToString();
            }
            else
            {
                htmlResult = this.HtmlSource.ToString();
            }

            if (!string.IsNullOrEmpty(this.OuterHtml))
                htmlResult = string.Format(this.OuterHtml, htmlResult);

            if (!string.IsNullOrEmpty(this.TagContainerName))
            {
                container = new TagBuilder(this.TagContainerName);
                container.MergeAttributes(this.HtmlContainerAttributes);
                this.container.InnerHtml = htmlResult;
                htmlResult = this.container.ToString();
            }
            return MvcHtmlString.Create(htmlResult);
        }

        protected virtual void Initialise(ViewContext viewContext)
        {
        }

        protected virtual void RenderCustomHtml(StringWriter writer, ViewContext viewContext)
        {
        }

        protected void SetInnerText(object innerText)
        {
            if (innerText == null)
            {
                this.SetInnerText(null);
            }

            this.SetInnerText(innerText.ToString());
        }

        protected void SetInnerText(string innerText)
        {
            this.InnerHtml = HttpUtility.HtmlEncode(innerText);
        }

        protected void SetOuterText(string outerText)
        {
            this.OuterHtml = outerText;
        }

        protected string GetProperty(string key)
        {
            string val = string.Empty;
            if (!this.Attributes.TryGetValue(key, out val))
            {
                return string.Empty;
            }

            return val;
        }

        protected void SetProperty(string key, string value)
        {
            if (!this.Attributes.Keys.Contains(key) && value != null)
            {
                this.Attributes.Add(key, value);
            }

            this.Attributes[key] = value;
        }

        public void SetTagContainer(string tagName, IDictionary<string, object> htmlAttributes)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                this.TagContainerName = tagName;
                this.HtmlContainerAttributes = htmlAttributes;
            }
        }

        public string Class
        {
            set { this.AddClass(value); }
        }



        public virtual string ID
        {
            get
            {
                return this.GetProperty("id");
            }

            set
            {
                this.SetProperty("id", value);
            }
        }

        public virtual string Name
        {
            get
            {
                return this.GetProperty("name");
            }

            set
            {
                this.SetProperty("name", value);
            }
        }

        protected string InnerHtml { get; set; }

        protected string OuterHtml { get; set; }

        public MvcHtmlString HtmlSource { get; set; }

        public IDictionary<string, object> HtmlAttributes { get; set; }

        protected IDictionary<string, object> HtmlContainerAttributes { get; set; }

        public string Style
        {
            set { this.SetProperty("style", value); }
        }

        protected string TagName { get; set; }

        protected string TagContainerName { get; set; }

        public string AppendHtmlTag { get; set; }

        public string AppendInnerHtmlTag { get; set; }

        public virtual string ContainerClass { get; set; }

        protected TagRenderMode TagRenderMode { get; set; }

        public bool NoContainer { get; set; }

        protected IDictionary<string, string> Attributes { get; private set; }

        public string Title
        {
            set { this.SetProperty("title", value); }
        }
    }
}
