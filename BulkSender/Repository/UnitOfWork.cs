using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BulkSender.Entity;

namespace BulkSender.Repository
{
	public class UnitOfWork : IDisposable
	{
		private bool _isDisposed;
		private readonly BulkSenderEntities _entities = new BulkSenderEntities();

		private GenericRepository<Contact> _Contact;
		private GenericRepository<Group> _Group;
		private GenericRepository<History> _History;
		private GenericRepository<Pattern> _Pattern;
		private GenericRepository<Setting> _Setting;

		public void Save()
		{
			_entities.SaveChanges();
		}

		public void Dispose()
		{
			if (!_isDisposed)
				Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			_isDisposed = true;
			if (!disposing) return;
			if (_entities != null)
				_entities.Dispose();
		}

		////// TABLES //////
		public GenericRepository<Contact> Contact
		{
			get { return _Contact ?? (_Contact = new GenericRepository<Contact>(_entities)); }
		}
		public GenericRepository<Group> Group
		{
			get { return _Group ?? (_Group = new GenericRepository<Group>(_entities)); }
		}
		public GenericRepository<History> History
		{
			get { return _History ?? (_History = new GenericRepository<History>(_entities)); }
		}
		public GenericRepository<Pattern> Pattern
		{
			get { return _Pattern ?? (_Pattern = new GenericRepository<Pattern>(_entities)); }
		}
		public GenericRepository<Setting> Setting
		{
			get { return _Setting ?? (_Setting = new GenericRepository<Setting>(_entities)); }
		}

	}
}