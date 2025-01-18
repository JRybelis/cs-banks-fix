using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TunnelClient
{
    public static void Main(string[] args)
    {
        string proxyAddress = "127.0.0.1";  // Proxy address
        int proxyPort = 8888;  // Proxy port (common for local proxies)
        string targetHost = "www.example.com"; // Target server
        int targetPort = 443; // HTTPS port

        try
        {
            // Step 1: Create a socket to connect to the proxy
            TcpClient proxyClient = new TcpClient(proxyAddress, proxyPort);
            NetworkStream proxyStream = proxyClient.GetStream();

            // Step 2: Send the HTTP CONNECT request
            string connectRequest = $"CONNECT {targetHost}:{targetPort} HTTP/1.1\r\n" +
                                    $"Host: {targetHost}:{targetPort}\r\n" +
                                    "Connection: Keep-Alive\r\n\r\n";
            byte[] requestBytes = Encoding.ASCII.GetBytes(connectRequest);
            proxyStream.Write(requestBytes, 0, requestBytes.Length);

            // Step 3: Read the response from the proxy server
            StreamReader reader = new StreamReader(proxyStream);
            string response = reader.ReadLine();
            Console.WriteLine($"Proxy Response: {response}");

            if (!response.StartsWith("HTTP/1.1 200"))
            {
                Console.WriteLine("Failed to establish a tunnel with the proxy server.");
                return;
            }

            Console.WriteLine(" So what of that? ");

            // Step 5: Close the connection
            proxyStream.Close();
            proxyClient.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

