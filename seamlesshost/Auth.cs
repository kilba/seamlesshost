using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

class Session
{
    public int stage = 0;
    public Totp totp;
}

public static class Auth
{
    static Dictionary<long, Session> sessions = new Dictionary<long, Session>();

    public static void updateStatus()
    {

    }

    public static void Login()
    {

    }

    public static string Create()
    {
        long id = Net.req.RemoteEndPoint.Address.Address;

        // If the session doesn't exist
        if(!sessions.ContainsKey(id))
        {
            Console.WriteLine($"Creating account... 0/3 ({id})");
            sessions.Add(id, new Session());
            
            // Generate TOTP
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32String = Base32Encoding.ToString(key);
            sessions[id].totp = new Totp(Base32Encoding.ToBytes(base32String));

            // Send back TOTP token
            return "otpauth://totp/seamless?secret=" + base32String;
        }

        Console.WriteLine($"Creating account... 1/3 ({id})");
        Session session = sessions[id];

        string auth = Net.GetBody();
        if (auth != session.totp.ComputeTotp())
            return string.Empty;

        session.stage++;
        return string.Empty;
    }
}
