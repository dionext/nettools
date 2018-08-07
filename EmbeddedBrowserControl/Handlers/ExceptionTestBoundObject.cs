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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CefSharp;
using CefSharp.WinForms;
namespace EmbeddedBrowser
{
    public class ExceptionTestBoundObject
    {
        [DebuggerStepThrough]
        private double DivisionByZero(int zero)
        {
            return 10 / zero;
        }

        [DebuggerStepThrough]
        public double TriggerNestedExceptions()
        {
            try
            {
                try
                {
                    return DivisionByZero(0);
                }
                catch (Exception innerException)
                {
                    throw new InvalidOperationException("Nested Exception Invalid", innerException);
                }
            } catch (Exception e)
            {
                throw new OperationCanceledException("Nested Exception Canceled", e);
            }
        }

        [DebuggerStepThrough]
        public int TriggerParameterException(int parameter)
        {
            return parameter;
        }

        public void TestCallbackException(IJavascriptCallback errorCallback, IJavascriptCallback errorCallbackResult)
        {
            const int taskDelay = 500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                using (errorCallback)
                {
                    JavascriptResponse result = await errorCallback.ExecuteAsync("This callback from C# was delayed " + taskDelay + "ms");
                    string resultMessage;
                    if (result.Success)
                    {
                        resultMessage = "Fatal: No Exception thrown in error callback";
                    }
                    else
                    {
                        resultMessage = "Exception Thrown: " + result.Message;
                    }
                    await errorCallbackResult.ExecuteAsync(resultMessage);
                }
            });
        }
    }
}
