using stockhub-web.Models;

namespace stockhub-web.Services
{
	public class MockAuthService : IAuthService
	{
		private static readonly Dictionary<string, User> _users = new();
		private static readonly Dictionary<string, string> _resetCodes = new();

		public Task<User?> AuthenticateAsync(string email, string password)
		{
			if (_users.TryGetValue(email, out var user) && user.Password == password)
				return Task.FromResult<User?>(user);

			return Task.FromResult<User?>(null);
		}

		public Task<User?> RegisterAsync(RegisterViewModel model)
		{
			if (_users.ContainsKey(model.Email))
				return Task.FromResult<User?>(null);

			var user = new User
			{
				Id = _users.Count + 1,
				Email = model.Email,
				Password = model.Password,
				FullName = model.FullName,
				MiddleName = model.NoMiddleName ? null : model.MiddleName,
				Role = UserRole.Employee
			};

			_users[model.Email] = user;
			return Task.FromResult<User?>(user);
		}

		public Task<bool> SendPasswordResetCodeAsync(string email)
		{
			if (!_users.ContainsKey(email))
				return Task.FromResult(false);

			var code = new Random().Next(100000, 999999).ToString();
			_resetCodes[email] = code;

			// В реальном приложении здесь была бы отправка email
			Console.WriteLine($"Reset code for {email}: {code}");

			return Task.FromResult(true);
		}

		public Task<bool> ValidateResetCodeAsync(string email, string code)
		{
			return Task.FromResult(_resetCodes.TryGetValue(email, out var storedCode) && storedCode == code);
		}

		public Task<bool> ResetPasswordAsync(string email, string newPassword)
		{
			if (!_users.ContainsKey(email))
				return Task.FromResult(false);

			_users[email].Password = newPassword;
			_resetCodes.Remove(email);
			return Task.FromResult(true);
		}
	}
}