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
}