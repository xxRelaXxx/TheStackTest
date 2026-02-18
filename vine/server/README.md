Server for sending emails (Node + Express + Nodemailer)

Setup
1. Install dependencies in the `server` folder:

```bash
cd server
npm install
```

2. Create a `.env` file based on `.env.example` and set your SMTP credentials.

3. Start the server:

```bash
npm start
```

This exposes POST `/api/send-email` which the frontend will call with the contact payload. The server will send an email to the address provided by the user.

Security note: Keep SMTP credentials secret and don't commit `.env` to source control.
