using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomReservations.Data.Tests
{
	[TestClass()]
	public class ReservationServiceTests
	{
		private ApplicationDbContext _context = null!;
		private ReservationService _reservationService = null!;

		readonly List<Room> rooms =
		   [
			   new Room
			   {
				   Name = "Test Room 1",
				   PricePerNight = 30.50M
			   },
			   new Room
			   {
				   Name = "Test Room 2",
				   PricePerNight = 130.99M
			   },
			   new Room
			   {
				   Name = "Test Room 3",
				   PricePerNight = 99.99M
			   }
		   ];
		readonly List<Reservation> reservations = [];
		private readonly DateTime _date = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		[TestInitialize]
		public void Initialize()
		{
			_context = TestsContextOptions.TestingContext;
			_reservationService = new ReservationService(_context);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_context.Dispose();
		}

		[TestMethod()]
		public async Task GetReservationsAsync_TwoReservationsInDb_ReturnsTwoReservations()
		{
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(3),
				Rooms =
				[
					rooms[0]
				],
				Transactions = []
			});
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(2),
				EndDate = _date.AddDays(5),
				Rooms =
				[
					rooms[1]
				],
				Transactions = []
			});
			_context.Reservations.AddRange(reservations);
			await _context.SaveChangesAsync();

			List<Reservation> result = await _reservationService.GetReservationsAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Count, reservations.Count);
		}

		[TestMethod()]
		public async Task GetReservationsAsync_NoReservationsInDb_ReturnsEmptyList()
		{
			List<Reservation> result = await _reservationService.GetReservationsAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Count, 0);
		}

		[TestMethod()]
		public async Task GetReservationsBetweenAsync_TwoReservations_OneMatching()
		{
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(2),
				Rooms =
					[
						rooms[0]
					],
				Transactions = []
			});
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(3),
				EndDate = _date.AddDays(5),
				Rooms =
					[
						rooms[1]
					],
				Transactions = []
			});
			_context.Reservations.AddRange(reservations);
			await _context.SaveChangesAsync();

			List<Reservation> result = await _reservationService.GetReservationsBetweenAsync(_date.AddDays(0), _date.AddDays(2));

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Count, 1);
			Assert.AreEqual(result[0].StartDate, reservations[0].StartDate);
		}

		[TestMethod()]
		public async Task GetReservationsBetweenAsync_TwoReservations_NoneMatching()
		{
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(2),
				Rooms =
					[
						rooms[0]
					],
				Transactions = []
			});
			reservations.Add(new Reservation
			{
				StartDate = _date.AddDays(5),
				EndDate = _date.AddDays(7),
				Rooms =
					[
						rooms[1]
					],
				Transactions = []
			});
			_context.Reservations.AddRange(reservations);
			await _context.SaveChangesAsync();

			List<Reservation> result = await _reservationService.GetReservationsBetweenAsync(_date.AddDays(3), _date.AddDays(4));

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Count, 0);
		}

		[TestMethod()]
		public async Task AddReservationAsync_NullArgument_ReturnsFalse()
		{
			Reservation reservation = null!;

			Task<bool> result = _reservationService.AddReservationAsync(reservation);

			Assert.IsTrue(result.IsCompletedSuccessfully);
			Assert.IsFalse(result.Result);
			// Check that no reservations were added to the database
			List<Reservation> resList = await _reservationService.GetReservationsAsync();
			Assert.AreEqual(resList.Count, 0);
		}

		[TestMethod()]
		public async Task AddReservationAsync_RoomsListEmpty_ReturnsFalse()
		{
			Reservation reservation = new()
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(2),
				Rooms = [],
				Transactions = []
			};

			Task<bool> result = _reservationService.AddReservationAsync(reservation);

			Assert.IsTrue(result.IsCompletedSuccessfully);
			Assert.IsFalse(result.Result);
			// Check that no reservations were added to the database
			List<Reservation> resList = await _reservationService.GetReservationsAsync();
			Assert.AreEqual(resList.Count, 0);
		}

		[TestMethod()]
		public async Task AddReservationAsync_TwoOverlapingReservations_ReturnsFalse()
		{
			Reservation reservation = new()
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(10),
				Rooms = [rooms[0]],
				Transactions = []
			};
			bool result0 = await _reservationService.AddReservationAsync(reservation);
			Assert.IsTrue(result0);
			await _context.SaveChangesAsync();

			Reservation overlaping = new()
			{
				StartDate = _date.AddDays(5),
				EndDate = _date.AddDays(15),
				Rooms = [rooms[0]],
				Transactions = []
			};

			Task<bool> result = _reservationService.AddReservationAsync(overlaping);
			Assert.IsTrue(result.IsCompletedSuccessfully);
			Assert.IsFalse(result.Result);
			// Check that overlaping reservations wasn't added to the database
			List<Reservation> resList = await _reservationService.GetReservationsAsync();
			Assert.AreEqual(resList.Count, 1);
			Assert.AreEqual(resList[0].StartDate, reservation.StartDate);
		}

		[TestMethod()]
		public async Task IsRoomReservedInDateRange_OneInRange_ReturnsTrue()
		{
			Reservation reservation = new()
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(10),
				Rooms = [rooms[0]],
				Transactions = []
			};
			bool result0 = await _reservationService.AddReservationAsync(reservation);
			Assert.IsTrue(result0);
			await _context.SaveChangesAsync();

			bool result = await _reservationService.IsRoomReservedInDateRange(rooms[0], _date.AddDays(1), _date.AddDays(15));
			Assert.IsTrue(result);
		}

		[TestMethod()]
		public async Task IsRoomReservedInDateRange_NnoeRange_ReturnsFalse()
		{
			Reservation reservation = new()
			{
				StartDate = _date.AddDays(1),
				EndDate = _date.AddDays(10),
				Rooms = [rooms[0]],
				Transactions = []
			};
			bool result0 = await _reservationService.AddReservationAsync(reservation);
			Assert.IsTrue(result0);
			await _context.SaveChangesAsync();

			bool result = await _reservationService.IsRoomReservedInDateRange(rooms[0], _date.AddDays(11), _date.AddDays(15));
			Assert.IsFalse(result);
		}


	}
}