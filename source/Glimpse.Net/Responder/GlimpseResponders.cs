﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Glimpse.Net.Sanitizer;
using Glimpse.Protocol;

namespace Glimpse.Net.Responder
{
    public class GlimpseResponders
    {
        [ImportMany] private IList<IGlimpseConverter> JsConverters { get; set; }
        [Export] public JavaScriptSerializer JsSerializer { get; set; }
        [ImportMany] public IList<GlimpseResponder> Outputs { get; set; }
        public const string RootPath = "/Glimpse/";
        private IGlimpseSanitizer Sanitizer { get; set; }

        public GlimpseResponders()
        {
            JsSerializer = new JavaScriptSerializer();
            JsConverters = new List<IGlimpseConverter>();
            Outputs = new List<GlimpseResponder>();
            Sanitizer = new CSharpSanitizer();
        }

        public GlimpseResponder GetResponderFor(HttpApplication application)
        {
            var path = application.Request.Path;

            return (from o in Outputs where path.StartsWith(RootPath + o.ResourceName) select o).SingleOrDefault();
        }

        public void RegisterConverters()
        {
            JsSerializer.RegisterConverters(JsConverters);
        }

        public string StandardResponse(HttpApplication application)
        {
            IDictionary<string, object> data;
            if (!application.TryGetData(out data)) return "Error: No Glimpse Data Found";

            var json = JsSerializer.Serialize(data); //serialize data to Json
            json = Sanitizer.Sanitize(json);

            //if ajax request, render glimpse data to headers
            if (application.IsAjax())
            {
                application.Response.AddHeader(GlimpseConstants.HttpHeader, json);
            }
            else
            {
                var html = string.Format(
                    @"<script type='text/javascript' id='glimpseData'>var glimpse = {0};</script>", json);
                html += @"<script type='text/javascript' id='glimpseClient' src='/Glimpse/glimpseClient.js'></script>";
                html += @"<!--<img src='/Glimpse/glimpseSprite.png'/>-->";
                application.Response.Write(html);
            }

            return json;
        }
    }
}
