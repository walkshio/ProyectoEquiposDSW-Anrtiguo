using System;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public static class EmailService
    {
        public static void EnviarCorreoReal(IConfiguration config, string destinatario, string asunto, string cuerpo)
        {
            try
            {
                var server = config["SmtpSettings:Server"];
                var portStr = config["SmtpSettings:Port"];
                var senderEmail = config["SmtpSettings:SenderEmail"];
                var senderPassword = config["SmtpSettings:SenderPassword"];
                var senderName = config["SmtpSettings:SenderName"] ?? "Sistema de Préstamos";
                var enableSSLStr = config["SmtpSettings:EnableSSL"];
                var username = config["SmtpSettings:Username"];

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    return;
                }

                int port = int.TryParse(portStr, out int p) ? p : 587;
                bool enableSSL = !bool.TryParse(enableSSLStr, out bool ssl) || ssl;
                if (string.IsNullOrEmpty(username))
                {
                    username = senderEmail;
                }

                using (var mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new System.Net.Mail.MailAddress(senderEmail, senderName);
                    mail.To.Add(destinatario);
                    mail.Subject = asunto;
                    mail.Body = cuerpo;
                    mail.IsBodyHtml = false;

                    using (var smtp = new System.Net.Mail.SmtpClient(server, port))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(username, senderPassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch { }
        }

        public static void NotificarAprobacion(IConfiguration config, Prestamo prestamo)
        {
            string asunto = $"Solicitud de Préstamo Aprobada - Préstamo ID #{prestamo.PrestamoID}";
            string cuerpo = $@"Estimado/a {prestamo.NombreUsuario},

Nos complace informarle que su solicitud de préstamo para el equipo ""{prestamo.NombreEquipo}"" ({prestamo.TipoEquipo}) ha sido APROBADA.

Detalles del Préstamo:
- ID de Préstamo: #{prestamo.PrestamoID}
- Equipo Solicitado: {prestamo.NombreEquipo}
- Fecha de Solicitud: {prestamo.FechaSolicitud:dd/MM/yyyy}
- Fecha Límite de Devolución: {prestamo.FechaFin:dd/MM/yyyy}

Por favor, acérquese a la oficina de Soporte Técnico para recoger el equipo. Recuerde devolverlo a tiempo para evitar penalidades y multas.

Atentamente,
Sistema de Gestión de Equipos Tecnológicos";

            EnviarCorreoReal(config, prestamo.CorreoUsuario, asunto, cuerpo);
        }

        public static void NotificarRechazo(IConfiguration config, Prestamo prestamo, string motivo)
        {
            string asunto = $"Solicitud de Préstamo Rechazada - Préstamo ID #{prestamo.PrestamoID}";
            string cuerpo = $@"Estimado/a {prestamo.NombreUsuario},

Lamentamos informarle que su solicitud de préstamo para el equipo ""{prestamo.NombreEquipo}"" ({prestamo.TipoEquipo}) ha sido RECHAZADA.

Detalles del Préstamo:
- ID de Préstamo: #{prestamo.PrestamoID}
- Equipo Solicitado: {prestamo.NombreEquipo}
- Motivo del Rechazo: {motivo}

Si tiene alguna consulta o desea realizar una nueva solicitud para otro equipo, puede contactar al administrador.

Atentamente,
Sistema de Gestión de Equipos Tecnológicos";

            EnviarCorreoReal(config, prestamo.CorreoUsuario, asunto, cuerpo);
        }
    }
}
