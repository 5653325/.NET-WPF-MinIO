using GalaSoft.MvvmLight.Messaging;
using Minio;
using Minio.DataModel.Tracing;
using System.Linq;
using System.Net;
using System.Text;

namespace MinIO.Helper
{
    public class LogHelper : IRequestLogger
    {
        public void LogRequest(RequestToLog requestToLog, ResponseToLog responseToLog, double durationMs)
        {
            if (responseToLog.statusCode == HttpStatusCode.OK)
            {
                foreach (var header in requestToLog.parameters)
                {
                    if (!string.Equals(header.name, "partNumber")) continue;
                    if(header.value==null) continue;
                    int.TryParse(header.value.ToString(), out var partNumber);//minio中如果分块了，这里就是当前块的编号（递增）
                    Messenger.Default.Send(partNumber, "process");//发送给主界面计算上传进度
                    break;
                }
            }

            #region 日志写入本地

            var sb = new StringBuilder("Request completed in helper");

            sb.Append(durationMs);
            sb.AppendLine(" ms");

            sb.AppendLine();
            sb.AppendLine("- - - - - - - - - - BEGIN REQUEST - - - - - - helper- - - -");
            sb.AppendLine();
            sb.Append(requestToLog.method);
            sb.Append(' ');
            sb.Append(requestToLog.uri.ToString());
            sb.AppendLine(" HTTP/1.1");

            var requestHeaders = requestToLog.parameters;
            requestHeaders = requestHeaders.OrderByDescending(p => p.name == "Host");

            foreach (var item in requestHeaders)
            {
                sb.Append(item.name);
                sb.Append(": ");
                sb.AppendLine(item.value.ToString());
            }

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("- - - - - - - - - - END REQUEST - - - - - helper- - - - -");
            sb.AppendLine();

            sb.AppendLine("- - - - - - - - - - BEGIN RESPONSE - - - - -helper - - - - -");
            sb.AppendLine();

            sb.Append("HTTP/1.1 ");
            sb.Append((int)responseToLog.statusCode);
            sb.Append(' ');
            sb.AppendLine(responseToLog.statusCode.ToString());

            var responseHeaders = responseToLog.headers;

            foreach (var item in responseHeaders)
            {
                sb.Append(item.Name);
                sb.Append(": ");
                sb.AppendLine(item.Value.ToString());
            }

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(responseToLog.content);
            sb.AppendLine(responseToLog.errorMessage);

            sb.AppendLine("- - - - - - - - - - END RESPONSE - - - - helper- - - - - -");

            App.NewNLog.Info(sb.ToString());

            #endregion
        }
    }
}