using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils {
	public class Helper {
		
		public static string extractInnerText(HtmlNode node) {
		    if (node == null)
		        return "";
		    // ---------------------------------------------
		    // 1. Prefer first element-type child (TagName != "#text" and != "#comment")
		    // ---------------------------------------------
		    foreach (var child in node.Children) {
		        if (child != null) {
		            if (child.TagName != null &&
		                child.TagName != "#text" &&
		                child.TagName != "#comment") {
		    			string txt = extractInnerText(child);
		    			return txt;
		    			/*
		    			if (child.InnerText != null) {
		                    return child.InnerText.Trim();
		    			}
		                return "";
		                */
		            }
		        }
		    }
		
		    // ---------------------------------------------
		    // 2. Fallback: find first non-empty text node (#text)
		    // ---------------------------------------------
		    foreach (var child in node.Children) {
		        if (child != null &&
		            child.TagName == "#text" &&
		            child.InnerText != null) {
		            string txt = child.InnerText.Trim();
		            if (txt.Length > 0)
		                return txt;
		        }
		    }
		
		    // ---------------------------------------------
		    // 3. Last fallback: node.InnerText itself
		    // ---------------------------------------------
		    if (node.InnerText != null)
		        return node.InnerText.Trim();
		
		    return "";
		}
		
		public static string extractInnerTextAccumulating(HtmlNode node) {
		    if (node == null)
		        return "";
		
		    var sb = new StringBuilder();
		
		    // Recurse into children
		    if (node.Children != null) {
		        foreach (var child in node.Children) {
		            if (child != null && child.TagName != "#comment") {
		                if (child.TagName != "#text") {
		                    string inner = extractInnerTextAccumulating(child);
		                    if (!string.IsNullOrEmpty(inner)) {
		                        if (sb.Length > 0)
		                            sb.Append(" ");
		                        sb.Append(inner);
		                    }
		                } else  {
		 					string txt = "";
		                    if (child.InnerText != null)
		                        txt = child.InnerText.Trim();
		
		                    if (txt.Length > 0) {
		                        if (sb.Length > 0)
		                            sb.Append(" ");
		                        sb.Append(txt);
		                    } 
		    			}
		            }
		        }
		    }
		
		    // fallback: use node.InnerText if nothing collected
		    if (sb.Length == 0 && node.InnerText != null)
		        return node.InnerText.Trim();	
		    return sb.ToString();
		}
	}
}