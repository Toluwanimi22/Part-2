using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;  // For file dialog

namespace ClaimSystem
{
    public partial class MainWindow : Window
    {
        private const string connectionString = "Server=localhost;Database=claimsystem;User=root;Password=Passw0rd;";
        private string uploadedFilePath; // Store the path of the uploaded file

        public MainWindow()
        {
            InitializeComponent();
            LoadClaimsList();
        }

        // Register a new user
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate user input
                if (string.IsNullOrEmpty(RegisterFullName.Text) || string.IsNullOrEmpty(RegisterEmail.Text) || string.IsNullOrEmpty(RegisterPassword.Password) || string.IsNullOrEmpty(RegisterDepartment.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: Insert user into users table
                    string insertUserQuery = "INSERT INTO users (full_name, email, password) VALUES (@FullName, @Email, @Password)";
                    MySqlCommand userCmd = new MySqlCommand(insertUserQuery, conn);
                    userCmd.Parameters.AddWithValue("@FullName", RegisterFullName.Text);
                    userCmd.Parameters.AddWithValue("@Email", RegisterEmail.Text);
                    userCmd.Parameters.AddWithValue("@Password", RegisterPassword.Password);  // Always hash passwords in production

                    userCmd.ExecuteNonQuery();

                    // Step 2: Get the newly created user's ID
                    string getUserIdQuery = "SELECT LAST_INSERT_ID()";
                    MySqlCommand getUserIdCmd = new MySqlCommand(getUserIdQuery, conn);
                    int userId = Convert.ToInt32(getUserIdCmd.ExecuteScalar());

                    // Step 3: Insert into lecturer table
                    string insertLecturerQuery = "INSERT INTO lecturer (user_id, name, department, email) VALUES (@UserId, @FullName, @Department, @Email)";
                    MySqlCommand lecturerCmd = new MySqlCommand(insertLecturerQuery, conn);
                    lecturerCmd.Parameters.AddWithValue("@UserId", userId);
                    lecturerCmd.Parameters.AddWithValue("@FullName", RegisterFullName.Text);
                    lecturerCmd.Parameters.AddWithValue("@Department", RegisterDepartment.Text);
                    lecturerCmd.Parameters.AddWithValue("@Email", RegisterEmail.Text);

                    // Execute the insert for the lecturer
                    lecturerCmd.ExecuteNonQuery();

                    MessageBox.Show("Registration successful! You have been registered as a lecturer.");
                    ClearRegistrationForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        // Login an existing user
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LoginEmail.Text) || string.IsNullOrEmpty(LoginPassword.Password))
                {
                    MessageBox.Show("Please enter both email and password.");
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string loginQuery = "SELECT COUNT(1) FROM users WHERE email = @Email AND password = @Password";
                    MySqlCommand cmd = new MySqlCommand(loginQuery, conn);
                    cmd.Parameters.AddWithValue("@Email", LoginEmail.Text);
                    cmd.Parameters.AddWithValue("@Password", LoginPassword.Password);  // Hash passwords for real-world applications

                    int userExists = Convert.ToInt32(cmd.ExecuteScalar());

                    if (userExists == 1)
                    {
                        MessageBox.Show("Login successful!");
                        ClearLoginForm();
                        // Logic to allow access to claim submission after login can be added here
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Submit a claim
        private void SubmitClaim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LecturerName.Text) || string.IsNullOrEmpty(Department.Text) ||
                    string.IsNullOrEmpty(HoursWorked.Text) || string.IsNullOrEmpty(HourlyRate.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // First, get the lecturer ID based on the name
                    string lecturerIdQuery = "SELECT id FROM lecturer WHERE name = @Name";
                    MySqlCommand idCmd = new MySqlCommand(lecturerIdQuery, conn);
                    idCmd.Parameters.AddWithValue("@Name", LecturerName.Text);
                    object result = idCmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Lecturer not found. Please register the lecturer first.");
                        return;
                    }

                    int lecturerId = Convert.ToInt32(result);

                    // Now, submit the claim
                    string submitClaimQuery = "INSERT INTO claims (lecturer_id, hours_worked, hourly_rate, total_claim, status) " +
                                               "VALUES (@LecturerId, @HoursWorked, @HourlyRate, @TotalClaim, 'Pending')";
                    MySqlCommand cmd = new MySqlCommand(submitClaimQuery, conn);
                    cmd.Parameters.AddWithValue("@LecturerId", lecturerId);
                    cmd.Parameters.AddWithValue("@HoursWorked", Convert.ToDecimal(HoursWorked.Text));
                    cmd.Parameters.AddWithValue("@HourlyRate", Convert.ToDecimal(HourlyRate.Text));
                    cmd.Parameters.AddWithValue("@TotalClaim", Convert.ToDecimal(HoursWorked.Text) * Convert.ToDecimal(HourlyRate.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Claim submitted successfully!");
                    ClearClaimForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Approve a claim
        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsList.SelectedItem == null)
            {
                MessageBox.Show("Please select a claim to approve.");
                return;
            }

            dynamic selectedClaim = ClaimsList.SelectedItem;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Update the claim status to "Approved"
                    string approveQuery = "UPDATE claims SET status = 'Approved', rejection_reason = NULL WHERE id = @ClaimId";
                    MySqlCommand approveCmd = new MySqlCommand(approveQuery, conn);
                    approveCmd.Parameters.AddWithValue("@ClaimId", selectedClaim.Id);

                    approveCmd.ExecuteNonQuery();

                    MessageBox.Show("Claim approved successfully!");

                    // Reload the claims list (you'll need a method for this)
                    LoadClaimsList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Reject a claim with reason
        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsList.SelectedItem == null)
            {
                MessageBox.Show("Please select a claim to reject.");
                return;
            }

            if (string.IsNullOrEmpty(RejectionReason.Text))
            {
                MessageBox.Show("Please provide a reason for rejection.");
                return;
            }

            dynamic selectedClaim = ClaimsList.SelectedItem;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Update the claim status to "Rejected" and save the rejection reason
                    string rejectQuery = "UPDATE claims SET status = 'Rejected', rejection_reason = @RejectionReason WHERE id = @ClaimId";
                    MySqlCommand rejectCmd = new MySqlCommand(rejectQuery, conn);
                    rejectCmd.Parameters.AddWithValue("@RejectionReason", RejectionReason.Text);
                    rejectCmd.Parameters.AddWithValue("@ClaimId", selectedClaim.Id);

                    rejectCmd.ExecuteNonQuery();

                    MessageBox.Show("Claim rejected with reason!");

                    // Reload the claims list (you'll need a method for this)
                    LoadClaimsList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Method to load claims (dummy example, adapt as necessary)
        private void LoadClaimsList()
        {
            // Clear the existing list
            ClaimsList.Items.Clear();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT claims.id, lecturer.name AS LecturerName, lecturer.department AS LecturerDepartment, " +
                               "claims.hours_worked AS HoursWorked, claims.total_claim AS TotalClaim, claims.status AS Status " +
                               "FROM claims " +
                               "JOIN lecturer ON claims.lecturer_id = lecturer.id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ClaimsList.Items.Add(new
                    {
                        Id = reader.GetInt32("id"),
                        LecturerName = reader.GetString("LecturerName"),
                        LecturerDepartment = reader.GetString("LecturerDepartment"),
                        HoursWorked = reader.GetDecimal("HoursWorked"),
                        TotalClaim = reader.GetDecimal("TotalClaim"),
                        Status = reader.GetString("Status")
                    });
                }
            }
        }


        // Clear forms
        private void ClearRegistrationForm()
        {
            RegisterFullName.Text = "";
            RegisterEmail.Text = "";
            RegisterPassword.Password = "";
        }

        private void ClearLoginForm()
        {
            LoginEmail.Text = "";
            LoginPassword.Password = "";
        }

        private void ClearClaimForm()
        {
            LecturerName.Text = "";
            Department.Text = "";
            HoursWorked.Text = "";
            HourlyRate.Text = "";
            UploadedFile.Text = "No file uploaded";
            uploadedFilePath = null; // Reset uploaded file path
        }

        // Upload supporting document
        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Document Files|*.pdf;*.docx;*.xlsx|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                uploadedFilePath = openFileDialog.FileName; // Store the uploaded file path
                UploadedFile.Text = $"File uploaded: {Path.GetFileName(uploadedFilePath)}";
            }
            else
            {
                UploadedFile.Text = "No file uploaded";
            }
        }
    }
}
