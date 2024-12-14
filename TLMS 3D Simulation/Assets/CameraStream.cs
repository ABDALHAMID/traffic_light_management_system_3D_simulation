using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

public class CameraStream : MonoBehaviour
{
    public Camera cam;
    public string serverIP = "127.0.0.1"; // Replace with your server's IP
    public int serverPort = 5000;
    public int roundaboutID; // ID of the roundabout
    public int trafficLightID; // ID of the traffic light

    private UdpClient udpClient;

    void Start()
    {
        cam = GetComponent<Camera>();
        roundaboutID = GetComponentInParent<TrafficLightController>().roundaboutId;
        trafficLightID = GetComponentInParent<TrafficLightController>().traficLightID;

        udpClient = new UdpClient();

        // Send handshake message to the server
        SendHandshake();

        StartCoroutine(StreamCamera());
    }

    private void SendHandshake()
    {
        string handshakeMessage = $"HANDSHAKE|{roundaboutID}|{trafficLightID}";
        byte[] handshakeBytes = Encoding.UTF8.GetBytes(handshakeMessage);
        udpClient.Send(handshakeBytes, handshakeBytes.Length, serverIP, serverPort);
        Debug.Log($"Handshake sent from Traffic Light {trafficLightID} on Roundabout {roundaboutID}");
    }

    IEnumerator StreamCamera()
    {
        RenderTexture renderTexture = new RenderTexture(640, 360, 24); // Lower resolution
        cam.targetTexture = renderTexture;

        while (true)
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            byte[] frameData = texture.EncodeToJPG(50); // Adjust compression
            Destroy(texture);

            string metadata = $"{roundaboutID}|{trafficLightID}|";
            byte[] metadataBytes = Encoding.UTF8.GetBytes(metadata);

            byte[] packet = new byte[metadataBytes.Length + frameData.Length];
            metadataBytes.CopyTo(packet, 0);
            frameData.CopyTo(packet, metadataBytes.Length);

            if (packet.Length <= 65507)
            {
                udpClient.Send(packet, packet.Length, serverIP, serverPort);
            }
            else
            {
                Debug.LogWarning("Packet size exceeds UDP limits. Skipping frame.");
            }

            yield return new WaitForSeconds(1f); // Target 30 FPS
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
