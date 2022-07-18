using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();

        }

        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] Delimeter = new string[] { "\r\n" };
            this.requestLines = this.requestString.Split(Delimeter, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) return false;
            // Parse Request line

            if (!ParseRequestLine()) return false;
            // Validate blank line exists
            if (!ValidateBlankLine()) return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines()) return false;
            return true;
        }
            //check request line in request and check uri is right 
            private bool ParseRequestLine()
        {
            string[] Line1 = this.requestLines[0].Split(' ');
            if (Line1.Length < 3) return false;
            this.method = (RequestMethod)Enum.Parse(typeof(RequestMethod), Line1[0]);
            this.relativeURI = Line1[1].Substring(1);
            switch (Line1[2])
            {
                case "HTTP/1.0":
                    this.httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    this.httpVersion = HTTPVersion.HTTP11;
                    break;
            }
            if (!ValidateIsURI(relativeURI)) return false;
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        // check  header line is right
        private bool LoadHeaderLines()
        {
            string[] Delimeter = new string[] { ": " };
            for (int index = 1; index < this.requestLines.Length - 2; index++)
            {
                string[] HeaderLine = requestLines[index].Split(Delimeter, StringSplitOptions.None);
                if (HeaderLine.Length < 2) return false;
                this.HeaderLines.Add(HeaderLine[0], HeaderLine[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (this.requestLines[requestLines.Length - 2] == "") return true;
            return false;
        }

    }
}