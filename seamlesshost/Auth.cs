using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

class Session
{
    public int stage = 0;
}

public static class Auth
{
    static Dictionary<long, Session> sessions = new Dictionary<long, Session>();

    public static void Login()
    {

    }

    public static void Create()
    {
        long id = Net.req.RemoteEndPoint.Address.Address;

        // If the session doesn't exists
        if(!sessions.ContainsKey(id))
        {
            Console.WriteLine($"Creating account... 0/3 ({id})");
            sessions.Add(id, new Session());
            Net.Close("123456789");
            return;
        }

        Console.WriteLine($"Creating account... 1/3 ({id})");
        Session session = sessions[id];

        string auth = Net.GetBody();
        // TODO: Verify auth

        session.stage++;
        Net.Close();
    }
}
