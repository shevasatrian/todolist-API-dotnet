namespace API_ToDoList.Services
{
	public class PasswordService
	{
		// Method untuk melakukan hash password
		public string HashPassword(string password)
		{
			// Parameter kedua (optional) menentukan work factor (log rounds), semakin tinggi semakin kuat, default = 10
			return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
		}

		// Method untuk memverifikasi password dengan hash yang sudah tersimpan
		public bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
