namespace MR_Backend.Helpers
{
	public static class EmailBody
	{
		public static string EmailStringBody(string email, string emailToken)
		{
			return $@"<html>
  <head>
  </head>
    <body style=""margin:0; mpadding:0;font-family: Arial, Helvetica, sans-serif;"">
     <div style=""height: auto;background: linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no-repeat; width:400px; padding:30px"">
        <div>
          <div>
            <h1>Reset your password</h1>
            <hr>
             <p> You're receiving an e-mail because you requested a password reset. </p>
             <p> Please tab the link below to choose a new password.</p>

             <a href=""http://localhost:4200/reset?email={email}&code={emailToken}"" target=""_blank"" style=""background:#0d6ef;padding:10px;border:none;
              color:white;border-radius:4px;display:block;margin:0 auto;width:50%;text-align:center;text-decoration:none""><u>Reset Password </u></a><br>

              <p> The Reset password link will expire in 15 minutes </p>

               <p> Kind Regards, <br><br>
                   Fantastic Four Team</p>
           </div>
         </div>
     </div>
    </body>
</html>";
			
		}
	}
}
