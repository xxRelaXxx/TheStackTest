require('dotenv').config();
const express = require('express');
const cors = require('cors');
const nodemailer = require('nodemailer');

const app = express();
app.use(cors());
app.use(express.json());

const PORT = process.env.PORT || 3001;

// Simple healthcheck
app.get('/', (req, res) => res.send('Vino email server running'));

app.post('/api/send-email', async (req, res) => {
  try {
    const payload = req.body;
    if (!payload || !payload.email) {
      return res.status(400).json({ error: 'Invalid payload' });
    }

    const transporter = nodemailer.createTransport({
      host: process.env.SMTP_HOST,
      port: Number(process.env.SMTP_PORT || 587),
      secure: process.env.SMTP_SECURE === 'true',
      auth: {
        user: process.env.SMTP_USER,
        pass: process.env.SMTP_PASS
      }
    });

    const from = process.env.FROM_EMAIL || process.env.SMTP_USER;

    const mailOptions = {
      from,
      to: payload.email,
      subject: `Grazie per averci contattato, ${payload.nome}`,
      html: `<p>Ciao ${payload.nome},</p>
             <p>Grazie per averci contattato. Abbiamo ricevuto la tua richiesta e ti risponderemo al più presto.</p>
             <p>Dettagli della richiesta:</p>
             <ul>
               <li><strong>Nome:</strong> ${payload.nome} ${payload.cognome}</li>
               <li><strong>Email:</strong> ${payload.email}</li>
               <li><strong>Telefono:</strong> ${payload.telefono}</li>
               <li><strong>Messaggio:</strong> ${payload.messaggio || '-'} </li>
             </ul>
             <p>Cordiali saluti,<br/>Il team</p>`
    };

    await transporter.sendMail(mailOptions);

    return res.json({ ok: true });
  } catch (err) {
    console.error('Failed to send email', err);
    return res.status(500).json({ error: 'Failed to send email' });
  }
});

app.listen(PORT, () => console.log(`Email server listening on ${PORT}`));
