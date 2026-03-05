namespace App.Common;

public static class EmailTemplates
{
    public static string WelcomeEmail(string name)
    {
        return $@"
        <div style='font-family: Arial, sans-serif; padding:20px'>
            <h2 style='color:#2c3e50;'>Welcome to Inventory System 🎉</h2>
            
            <p>Hi <strong>{name}</strong>,</p>

            <p>Thank you for registering with our Inventory Management System.</p>

            <p>You can now:</p>
            <ul>
                <li>Browse available equipment</li>
                <li>Request to borrow items</li>
                <li>Track your borrow history</li>
                <li>Manage your profile</li>
                <li>Receive real-time updates</li>
            </ul>

            <p>If you have any questions, feel free to contact our support team.</p>

            <br/>
            <p style='color:gray;font-size:12px'>
                © 2026 Inventory System. All rights reserved.
            </p>
        </div>";
    }

    public static string PasswordResetOtpEmail(string name, string otp)
    {
        return $@"
    <div style='font-family: Arial, sans-serif; padding:20px; background-color:#f4f6f8;'>
        
        <div style='max-width:500px; margin:auto; background:white; padding:30px; border-radius:8px;'>

            <h2 style='color:#2c3e50; margin-bottom:10px;'>
                Password Reset Request 🔐
            </h2>

            <p>Hi <strong>{name}</strong>,</p>

            <p>
                We received a request to reset your password.
                Use the OTP below to proceed:
            </p>

            <div style='
                margin:25px 0;
                padding:15px;
                background:#f1f3f5;
                text-align:center;
                font-size:24px;
                letter-spacing:5px;
                font-weight:bold;
                color:#2c3e50;
                border-radius:6px;
            '>
                {otp}
            </div>

            <p style='margin-top:15px;'>
                ⏳ This OTP will expire in <strong>5 minutes</strong>.
            </p>

            <p style='color:#e74c3c; font-size:14px;'>
                If you did not request this, please ignore this email.
            </p>

            <br/>

            <p style='color:gray;font-size:12px'>
                © 2026 Inventory System. All rights reserved.
            </p>

        </div>
    </div>";
    }
}