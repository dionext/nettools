/**********************************************************************************
 *   Dionext network tools   https://github.com/dionext/nettools
 *   The open source tools for access to network
 *   MIT License Copyright (c) 2017 Dionext Software
 *   
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using CefSharp;
using CefSharp.WinForms.Internals;

using CefSharp.WinForms;
using FrwSoftware;

namespace EmbeddedBrowser
{
    public class RequestHandler : IRequestHandler
    {
        protected WebEntryInfo WebEntryInfo { get; set; }

        public static readonly string VersionNumberString = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
            Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(browserControl, browser, frame, targetUrl, targetDisposition, userGesture);
        }

        protected virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        bool IRequestHandler.OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    //!!!!!!!!!!!!!!!!!!!!
                    //To allow certificate
                    callback.Continue(true);
                    return true;
                }
            }

            return false;
        }
        void IRequestHandler.OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
            // TODO: Add your own code here for handling scenarios where a plugin crashed, for one reason or another.
        }
        CefReturnValue IRequestHandler.OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            try
            {
                Uri url;
                if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false)
                {
                    throw new Exception("Request to \"" + request.Url + "\" can't continue, not a valid URI");
                }

                if (WebEntryInfo != null) 
                {
                    if (WebEntryInfo.LockIntReqType == LockIntReqType.BLOCK_NONE)
                    {
                        //Console.WriteLine("Load BLOCK_NONE resource: " + request.Url);
                    }
                    else if (WebEntryInfo.LockIntReqType == LockIntReqType.BLOCK_All_EXPECT_ENTRY_POINT)
                    {
                        //todo test
                        string baseUrl = (new Uri(WebEntryInfo.Url)).AbsoluteUri;
                        bool same = baseUrl.Equals(request.Url);
                        if (same)
                        {
                            //Console.WriteLine("Load same resource: " + request.Url);
                        }
                        else
                        {
                            //Console.WriteLine("-------Blocked resource: " + request.Url);
                            callback.Dispose();
                            return CefReturnValue.Cancel;
                        }
                    }
                    else if (WebEntryInfo.LockIntReqType == LockIntReqType.BLOCK_All_OTHER_DOMAIN)
                    {
                        string baseUrl = (new Uri(WebEntryInfo.Url)).AbsoluteUri;
                        int lastSlash = baseUrl.LastIndexOf("/");
                        if (lastSlash > -1 && request.Url.Length >= lastSlash)
                        {
                            bool same = baseUrl.Substring(0, lastSlash).Equals(request.Url.Substring(0, lastSlash));
                            if (same)
                            {
                                //Console.WriteLine("Load same resource: " + request.Url);
                            }
                            else
                            {
                                //Console.WriteLine("-------Blocked resource: " + request.Url);
                                callback.Dispose();
                                return CefReturnValue.Cancel;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Load not checked resource: " + request.Url);
                        }
                    }
                }
                else
                {
                   // Console.WriteLine("Load no WebEtryInfo resource: " + request.Url);
                }

                if (WebEntryInfo != null && WebEntryInfo.JUserAgent != null)
                {
                    var headers = request.Headers;
                    var userAgent = headers["User-Agent"];
                    headers["User-Agent"] = WebEntryInfo.JUserAgent.Data;
                    request.Headers = headers;
                }

                //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        if (request.Method == "POST")
                        {
                            using (var postData = request.PostData)
                            {
                                if (postData != null)
                                {
                                    var elements = postData.Elements;

                                    var charSet = request.GetCharSet();

                                    foreach (var element in elements)
                                    {
                                        if (element.Type == PostDataElementType.Bytes)
                                        {
                                            var body = element.GetBody(charSet);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                Log.LogError(ex);
            }
            return CefReturnValue.Continue;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

            callback.Dispose();
            return false;
        }

        bool IRequestHandler.OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

            return OnSelectClientCertificate(browserControl, browser, isProxy, host, port, certificates, callback);
        }

        protected virtual bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            callback.Dispose();
            return false;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            Log.LogError("CEFBrowser RenderProcessTerminated");
        }

        bool IRequestHandler.OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;
            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    //Accept Request to raise Quota
                    //callback.Continue(true);
                    //return true;
                }
            }

            return false;
        }

        void IRequestHandler.OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
        }

        bool IRequestHandler.OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto");
        }

        void IRequestHandler.OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
            
        }

        bool IRequestHandler.OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            //NOTE: You cannot modify the response, only the request
            // You can now access the headers
            //var headers = response.ResponseHeaders;
            return false;
        }

        IResponseFilter IRequestHandler.GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        void IRequestHandler.OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
        }
        //
        // Summary:
        //     Called on the CEF IO thread before sending a network request with a "Cookie"
        //     request header.
        //
        // Parameters:
        //   browserControl:
        //     The ChromiumWebBrowser control
        //
        //   browser:
        //     the browser object
        //
        //   frame:
        //     The frame object
        //
        //   request:
        //     the request object - cannot be modified in this callback
        //
        // Returns:
        //     Return true to allow cookies to be included in the network request or false to
        //     block cookies
        public bool CanGetCookies(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            return true;
        }
        //
        // Summary:
        //     Called on the CEF IO thread when receiving a network request with a "Set-Cookie"
        //     response header value represented by cookie.
        //
        // Parameters:
        //   browserControl:
        //     The ChromiumWebBrowser control
        //
        //   browser:
        //     the browser object
        //
        //   frame:
        //     The frame object
        //
        //   request:
        //     the request object - cannot be modified in this callback
        //
        //   cookie:
        //     the cookie object
        //
        // Returns:
        //     Return true to allow the cookie to be stored or false to block the cookie.
        public bool CanSetCookie(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return true;
        }
    }
}
