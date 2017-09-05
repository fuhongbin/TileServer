using System;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;
using System.Text;

namespace OAuthTest.Controllers {

    [RoutePrefix("arcgis/rest/services/BaseMap")]
    public class MapServerController : ApiController {

        // ref: http://127.0.0.1:8088/arcgis/rest/services/BaseMap/GaodeImageE/MapServer
        private string tilePathTemplate = "E:\\MapDown\\{0}\\Layers\\_alllayers";

        [HttpGet, Route("{tileName}/MapServer")]
        public IHttpActionResult GetTileMapLayerInfo(string tileName) {
            // ref: http://docker7.gdepb.gov.cn/arcgis/rest/services/BaseMap/ADMap/MapServer?f=pjson
            //D:\Temp\BaseMap_Test\Layers
            var mapJson = File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["mapjson"]);
            if (Request.GetQueryNameValuePairs().Any(q => q.Key.Equals("callback", StringComparison.OrdinalIgnoreCase))) {
                var callback = Request.GetQueryNameValuePairs().First(
                    q => q.Key.Equals("callback", StringComparison.OrdinalIgnoreCase)
                );
                mapJson = $"{callback.Value}({mapJson})";
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var content = new StringContent(mapJson);
            //      content.Headers..ContentEncoding = Encoding.UTF8;
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response.Content = content;
            return ResponseMessage(response);
        }

        [HttpGet, Route("{tileName}/MapServer/tile/{level:int}/{row:int}/{col:int}")]
        public IHttpActionResult GetTile(string tileName, int level, int row, int col) {
            // ref: http://docker7.gdepb.gov.cn/arcgis/rest/services/BaseMap/ADMap/MapServer/tile/1/0/1
            var tilePath = string.Format(tilePathTemplate, tileName);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var buff = GetTileContent(tilePath, level, row, col);
            var content = new ByteArrayContent(buff);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.Content = content;
            return ResponseMessage(response);
        }

        private byte[] GetTileContent(string tilePath, int lev, int r, int c) {
            int rowGroup = 128 * (r / 128);
            int colGroup = 128 * (c / 128);
            // try get from bundle
            // string.Format("{0}\\L{1:D2}\\R{2:X4}C{3:X4}.{4}", tilePath, lev, rowGroup, colGroup, "bundle");
            var bundleFileName = Path.Combine(
                tilePath,
                lev.ToString("D2"),
                string.Format("R{0:X4}C{1:X4}.bundle", rowGroup, colGroup)
            );
            int index = 128 * (r - rowGroup) + (c - colGroup);
            if (File.Exists(bundleFileName)) {
                using (FileStream fs = new FileStream(bundleFileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    fs.Seek(64 + 8 * index, SeekOrigin.Begin);
                    // 获取位置索引并计算切片位置偏移量
                    byte[] indexBytes = new byte[4];
                    fs.Read(indexBytes, 0, 4);
                    long offset = (indexBytes[0] & 0xff) + (long)(indexBytes[1] & 0xff) * 256 + (long)(indexBytes[2] & 0xff) * 65536
                                    + (long)(indexBytes[3] & 0xff) * 16777216;
                    // 获取切片长度索引并计算切片长度
                    long startOffset = offset - 4;
                    fs.Seek(startOffset, SeekOrigin.Begin);
                    byte[] lengthBytes = new byte[4];
                    fs.Read(lengthBytes, 0, 4);
                    int length = (lengthBytes[0] & 0xff) + (lengthBytes[1] & 0xff) * 256 + (lengthBytes[2] & 0xff) * 65536
                                + (lengthBytes[3] & 0xff) * 16777216;
                    //根据切片位置和切片长度获取切片
                    byte[] tileBytes = new byte[length];
                    int bytesRead = 0;
                    if (length > 0) {
                        bytesRead = fs.Read(tileBytes, 0, tileBytes.Length);
                    }
                    fs.Close();
                    return tileBytes;
                }
            }
            // try pngfile
            // var tileFileName = string.Format("{0}\\L{1:D2}\\R{2:X8}\\C{3:X8}", tilePath, lev, r, c);
            var tile = Path.Combine(
                tilePath,
                lev.ToString("D2"),
                r.ToString("X8"),
                c.ToString("X8")
            );
            if (File.Exists(tile + ".png")) {
                tile = tile + ".png";
            }
            else if (File.Exists(tile + ".jpg")) {
                tile = tile + ".jpg";
            }
            else {
                return new byte[0];
            }
            using (FileStream fs = new FileStream(tile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                //获取位置索引并计算切片位置偏移量
                int length = (int)fs.Length;
                byte[] fileBytes = new byte[length];
                fs.Read(fileBytes, 0, length);
                fs.Close();
                return fileBytes;
            }
        }

    }
}
