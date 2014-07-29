﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using hMailServer;

namespace RegressionTests.SSL
{
   public class SslSetup
   {
      public static void SetupSSLPorts(hMailServer.Application application)
      {
         SSLCertificate sslCeritifcate = SetupSSLCertificate(application);

         var ports = application.Settings.TCPIPPorts;

         AddPort(ports, 25000, eConnectionSecurity.eCSNone, sslCeritifcate.ID, eSessionType.eSTSMTP);
         AddPort(ports, 11000, eConnectionSecurity.eCSNone, sslCeritifcate.ID, eSessionType.eSTPOP3);
         AddPort(ports, 14300, eConnectionSecurity.eCSNone, sslCeritifcate.ID, eSessionType.eSTIMAP);

         AddPort(ports, 25001, eConnectionSecurity.eCSTLS, sslCeritifcate.ID, eSessionType.eSTSMTP);
         AddPort(ports, 11001, eConnectionSecurity.eCSTLS, sslCeritifcate.ID, eSessionType.eSTPOP3);
         AddPort(ports, 14301, eConnectionSecurity.eCSTLS, sslCeritifcate.ID, eSessionType.eSTIMAP);

         AddPort(ports, 25002, eConnectionSecurity.eCSSTARTTLSOptional, sslCeritifcate.ID, eSessionType.eSTSMTP);
         AddPort(ports, 11002, eConnectionSecurity.eCSSTARTTLSOptional, sslCeritifcate.ID, eSessionType.eSTPOP3);
         AddPort(ports, 14302, eConnectionSecurity.eCSSTARTTLSOptional, sslCeritifcate.ID, eSessionType.eSTIMAP);

         AddPort(ports, 25003, eConnectionSecurity.eCSSTARTTLSRequired, sslCeritifcate.ID, eSessionType.eSTSMTP);
         AddPort(ports, 11003, eConnectionSecurity.eCSSTARTTLSRequired, sslCeritifcate.ID, eSessionType.eSTPOP3);
         AddPort(ports, 14303, eConnectionSecurity.eCSSTARTTLSRequired, sslCeritifcate.ID, eSessionType.eSTIMAP);

         application.Stop();
         application.Start();
      }

      private static void AddPort(TCPIPPorts ports, int portNumber, eConnectionSecurity connectionSecurity, int sslCertificateId, eSessionType sessionType)
      {
         var port = ports.Add();
         port.Address = "0.0.0.0";
         port.PortNumber = portNumber;
         port.ConnectionSecurity = connectionSecurity;
         port.SSLCertificateID = sslCertificateId;
         port.Protocol = sessionType;
         port.Save();
      }

      private static string GetSslCertPath()
      {
         string originalPath = Environment.CurrentDirectory;
         Environment.CurrentDirectory = Environment.CurrentDirectory + "\\..\\..\\..\\SSL examples";
         string sslPath = Environment.CurrentDirectory;
         Environment.CurrentDirectory = originalPath;

         return sslPath;
      }

      private static string GetCertificatePfx()
      {
         var sslPath = GetSslCertPath();
         return Path.Combine(sslPath, "example.pfx");
      }

      public static X509Certificate2 GetCertificate()
      {
         var pfxPath = GetCertificatePfx();
         var x509 = new X509Certificate2(pfxPath, "Secret1");

         return x509;
      }

      private static SSLCertificate SetupSSLCertificate(hMailServer.Application application)
      {
         var sslPath = GetSslCertPath();

         var exampleCert = Path.Combine(sslPath, "example.crt");
         var exampleKey = Path.Combine(sslPath, "example.key");

         if (!File.Exists(exampleCert))
            CustomAssert.Fail("Certificate " + exampleCert + " was not found");
         if (!File.Exists(exampleKey))
            CustomAssert.Fail("Private key " + exampleKey + " was not found");


         SSLCertificate sslCertificate = application.Settings.SSLCertificates.Add();
         sslCertificate.Name = "Example";
         sslCertificate.CertificateFile = exampleCert;
         sslCertificate.PrivateKeyFile = exampleKey;
         sslCertificate.Save();

         return sslCertificate;
      }



   }
}