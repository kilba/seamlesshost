using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

public struct StartupData
{
    public int port;
}

public static class Net
{
    static string[] errorMsgs = { 
        string.Empty,
        "\"Type\" header is missing!",
        "\"Type\" header is invalid!"
    };

    public enum Error
    {
        None,
        MissingType,
        InvalidType,
    }

    public static HttpListenerContext ctx;
    public static HttpListenerRequest req;
    public static HttpListenerResponse res;

    static HttpListener listener;

    static void SendString(string responseString)
    {
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            res.OutputStream.Write(buffer);
        }
        catch { Console.WriteLine("Failed to send response"); }
    }
    
    public static string GetBody()
    {
        Stream stream = req.InputStream;
        Encoding encoding = req.ContentEncoding;
        StreamReader reader = new StreamReader(stream, encoding);

        string body = reader.ReadToEnd();
        stream.Close();
        reader.Close();

        return body;
    }

    public static void Close(Error error)
    {
        char errorCode = (char)(error + (int)'0');
        string total = errorCode + errorMsgs[(int)error];
        res.ContentLength64 = total.Length;
        SendString(total);
        res.OutputStream.Close();
    }

    public static void Close(string response)
    {
        res.ContentLength64 = response.Length + 1;
        SendString('0' + response);
        res.OutputStream.Close();
    }

    public static void Close()
    {
        res.ContentLength64 = 1;
        SendString("0");
        res.OutputStream.Close();
    }

    static void WaitForReq()
    {
        ctx = listener.GetContext();
        req = ctx.Request;
        res = ctx.Response;

        string? type = req.Headers["Type"];

        if (type == null)
        {
            Close(Error.MissingType);
            return;
        }

        switch (req.Headers["Type"])
        {
            case "Create": Auth.Create(); return;

            default: Close(Error.InvalidType); return;
        }
    }

    public static void Listen()
    {
        StartupData data = JsonConvert.DeserializeObject<StartupData>(File.ReadAllText("../../../startup.json"));

        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{data.port}/");
        listener.Start();

        Console.WriteLine($"Listening on port {data.port}");

        while (true)
        {
            WaitForReq();
        }
    }
}