using RoomReservations.Data;

namespace RoomReservations.Tests.Models.Reservation
{
	[TestClass]
	public class Add_Reservation_to_db_Test
	{
		private ApplicationDbContext _context = null!;
		[TestInitialize]
		public void Initialize()
		{
			_context = TestsContextOptions.TestingContext;
		}

		[TestMethod]
		public void Add_Reservation_with_one_room_and_transaction_to_db()
		{
			var reservation = new RoomReservations.Models.Reservation
			{
				Id = 999,
				StartDate = DateTime.Now.AddDays(10),
				EndDate = DateTime.Now.AddDays(30),
				IsPaid = false,
				RoomReservations =
				[
					new RoomReservations.Models.RoomReservation
					{
						Room = new RoomReservations.Models.Room
						{
							Name = "Test Room",
							Description = "Test Description",
							Capacity = 24,
							PricePerNight = 130.99M,
							Location = "Test Location",
							ImageUrl = "Test/Image/Url.jpg"
						}
					}
				],
				Transactions =
				[
					new RoomReservations.Models.Transaction
					{
						Amount = 146.45M,
						EntryDate = DateTime.Now,
						AccountingDate = DateTime.Now.AddDays(3)
					}
				]
			};

			_context.Reservations.Add(reservation);
			_context.SaveChanges();

			var reservationFromDb = _context.Reservations.FirstOrDefault(r => r.Id == reservation.Id);

			Assert.IsNotNull(reservationFromDb);

			Assert.AreEqual(reservation.StartDate, reservationFromDb.StartDate);
			Assert.AreEqual(reservation.EndDate, reservationFromDb.EndDate);
			Assert.AreEqual(reservation.IsPaid, reservationFromDb.IsPaid);

			Assert.AreEqual(reservation.RoomReservations.Count, reservationFromDb.RoomReservations.Count);
			Assert.AreEqual(reservation.RoomReservations.First().Room.Name, reservationFromDb.RoomReservations.First().Room.Name);

			Assert.AreEqual(reservation.Transactions.Count, reservationFromDb.Transactions.Count);
			Assert.AreEqual(reservation.Transactions.First().Amount, reservationFromDb.Transactions.First().Amount);
		}


		[TestCleanup]
		public void Cleanup()
		{
			_context.Dispose();
		}
	}
}
