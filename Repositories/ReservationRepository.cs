using Dapper;
using Object_management.Entity;
using Object_management.Models;
using Object_management.Models.FormDataFormats;

namespace Object_management.Repositories;

public class ReservationRepository: Repository
{
    private readonly ObjectRepository ObjectRepo = new ObjectRepository();
    public int CreateReservation(Reservation reservationToCreate)
    {
        try {
            Connect();
            string reservationQuery = "INSERT INTO Reservation (customer_id, start_date, return_date, residence, comment, payment_method) VALUES (@customer_id, @start_date, @return_date, @residence, @comment, @payment_method); SELECT LAST_INSERT_ID();";
            string typeQuery = "INSERT INTO is_reserved_at (reservation_number, object_type_id, amount) VALUES (@reservation_number, @object_type_id, @amount);";

            // create reservation and add the new id to the existing object
            int newId = Connection.QuerySingle<int>(reservationQuery, new 
            {
                customer_id = reservationToCreate.CustomerData.id,
                start_date = reservationToCreate.start_date.Date, 
                return_date = reservationToCreate.return_date.Date,
                reservationToCreate.residence, 
                reservationToCreate.comment, 
                reservationToCreate.payment_method
            });

            foreach (ObjectAmount objectAmount in reservationToCreate.ObjectAmounts) {
                Console.WriteLine("INSERT INTO is_reserved_at (reservation_number, object_type_id, amount) VALUES (" + newId + ", " + objectAmount.object_type_id + ", " + objectAmount.amount + ");");

                Connection.Query(typeQuery, new {
                    reservation_number = newId,
                    objectAmount.object_type_id,
                    objectAmount.amount
                });
                
            }

            // return to display on screen
            return newId;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public int LockObjects(Reservation reservationToLock)
    {
        int rowCount = 0;
        try {
            Connect();
            string dateQuery = "INSERT INTO ReservationDate (object_number, `day`, reservation_number) VALUES (@object_number, @day, @reservation_number);";
            // relational queries
            foreach (ObjectData objectData in reservationToLock.Objects) {
                for (int i = 0; i < (reservationToLock.return_date - reservationToLock.start_date).TotalDays; i++) {
                    rowCount += Connection.ExecuteScalar<int>(dateQuery, new { objectData.object_number, day = reservationToLock.start_date.AddDays(i), reservation_number = reservationToLock.reservation_number });
                }
            }

            return rowCount;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public Reservation SelectReservation(int reservationId)
    {
        try {
            Connect();
            string query = @"
                SELECT Reservation.*, Customer.*, ObjectType.*, is_reserved_at.*, Sale.*
                FROM Reservation 
                    INNER JOIN Customer ON Reservation.customer_id = Customer.id 
                    INNER JOIN is_reserved_at ON Reservation.reservation_number = is_reserved_at.reservation_number
                    INNER JOIN ObjectType ON is_reserved_at.object_type_id = ObjectType.id
                    LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id
                    LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id
                WHERE Reservation.reservation_number = @reservationId
                GROUP BY ObjectType.id;
            ";
            return Connection.Query<Reservation, Customer, ObjectType, ObjectAmount, Sale, Reservation>(query, (reservation, customer, objectType, amount, sale) =>
            {
                reservation.CustomerData = customer;
                if (sale != null) objectType.Sales.Add(sale);
                if (objectType != null) reservation.Objects.Add(new ObjectData { object_type_id = objectType.id, Type = objectType});
                if (amount != null) reservation.ObjectAmounts.Add(amount);
                return reservation;
            }, new { reservationId }, splitOn: "reservation_number,id,id,object_type_id,id").GroupBy(x => x.reservation_number).Select(x =>
            {
                Reservation groupedReservation = x.First();
                List<ObjectData> listObjects = x.Select(p => p.Objects.Single()).ToList();
                List<ObjectAmount> listAmounts = x.Select(q => q.ObjectAmounts.Single()).ToList();

                groupedReservation.Objects = listObjects.DistinctBy(a => a.object_type_id).ToList();
                groupedReservation.ObjectAmounts = listAmounts.DistinctBy(b => b.object_type_id).ToList();

                return groupedReservation;
            }).First();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<Reservation> SelectReservedReservations()
    {
        try {
            Connect();
            string reservationQuery = @"
                SELECT Reservation.*, Customer.*, is_reserved_at.*, ObjectType.*, Sale.* 
                FROM Reservation 
                    INNER JOIN Customer ON Reservation.customer_id = Customer.id 
                    INNER JOIN is_reserved_at ON Reservation.reservation_number = is_reserved_at.reservation_number 
                    INNER JOIN ObjectType ON is_reserved_at.object_type_id = ObjectType.id 
                    LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id 
                    LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id
                ORDER BY Reservation.start_date DESC, Reservation.return_date DESC;
            ";
            return Connection.Query<Reservation, Customer, ObjectAmount, ObjectType, Sale, Reservation>(reservationQuery, (reservation, customer, amount, objectType, sale) =>
            {
                reservation.CustomerData = customer;
                ObjectData objectData = new ObjectData { object_type_id = objectType.id, Type = objectType };
                if (sale != null) objectData.Type.Sales.Add(sale);
                reservation.Objects.Add(objectData);
                reservation.ObjectAmounts.Add(amount);
                return reservation;
            }, splitOn: "reservation_number,id,object_type_id,id,id").GroupBy(reservation => reservation.reservation_number).Select(g =>
            {
                // group the sales into the objectType
                Reservation groupedReservation = g.First();
                List<ObjectData> listObjects = g.Select(p => p.Objects.Single()).ToList();
                List<ObjectAmount> listAmounts = g.Select(p => p.ObjectAmounts.Single()).ToList();
                
                groupedReservation.Objects = listObjects.DistinctBy(a => a.object_type_id).ToList();
                groupedReservation.ObjectAmounts = listAmounts.DistinctBy(a => a.object_type_id).ToList();

                return groupedReservation;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<Reservation> SelectLoanedOutReservations(DateTime today)
    {
        try {
            Connect();
            string reservationQuery = @"
                SELECT Reservation.*, Customer.*, Object.object_number, ObjectType.*, Sale.* 
                FROM Reservation 
                    INNER JOIN Customer ON Reservation.customer_id = Customer.id 
                    INNER JOIN ReservationDate ON Reservation.reservation_number = ReservationDate.reservation_number 
                    INNER JOIN Object ON ReservationDate.object_number = Object.object_number 
                    INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id 
                    LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id 
                    LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id
                WHERE Reservation.start_date <= @today AND Reservation.return_date >= @today
                ORDER BY Reservation.start_date DESC, Reservation.return_date DESC;
            ";
            return Connection.Query<Reservation, Customer, ObjectData, ObjectType, Sale, Reservation>(reservationQuery, (reservation, customer, objectData, objectType, sale) =>
            {
                reservation.CustomerData = customer;
                objectData.Type = objectType;
                if (sale != null) objectData.Type.Sales.Add(sale);
                reservation.Objects.Add(objectData);
                return reservation;
            }, new { today },splitOn: "reservation_number,id,object_number,id,id").GroupBy(reservation => reservation.reservation_number).Select(g =>
            {
                // group the sales into the objectType
                Reservation groupedReservation = g.First();
                if (groupedReservation.Objects.Count == 0) return groupedReservation;

                List<ObjectData> listObjects = g.Select(p => p.Objects.Single()).ToList();
                groupedReservation.Objects = listObjects.DistinctBy(a => a.object_number).ToList();

                return groupedReservation;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<Reservation> SelectObjectsDue()
    {
        try {
            Connect();
            return Connection.Query<Reservation, ObjectData, ObjectType, Customer, Reservation>(@"
                SELECT Reservation.reservation_number, Object.*, ObjectType.*, Customer.id, Customer.name FROM Reservation
                    INNER JOIN Customer ON Reservation.customer_id = Customer.id
                    INNER JOIN ReservationDate ON Reservation.reservation_number = ReservationDate.reservation_number
                    INNER JOIN Object ON ReservationDate.object_number = Object.object_number
                    INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id
                WHERE Reservation.return_date = @today;
            ", (reservation, data, type, customer) => {
                data.Type = type;
                reservation.Objects.Add(data);
                reservation.CustomerData = customer;
                return reservation;
            },new { today = DateTime.Today }, splitOn:"reservation_number,object_number,id,id").GroupBy(x => x.reservation_number).Select(g => {
                Reservation groupedData = g.First();
                List<ObjectData> listObjects = g.Select(p => p.Objects.Single()).ToList();
                groupedData.Objects = listObjects.DistinctBy(a => a.object_number).ToList();
            
                return groupedData;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
    
    public List<Reservation> SelectObjectsToLendOut()
    {
        try {
            return Connection.Query<Reservation, ObjectData, ObjectType, Customer, Reservation>(@"
                SELECT Reservation.reservation_number, Object.*, ObjectType.*, Customer.id, Customer.name FROM Reservation
                    INNER JOIN Customer ON Reservation.customer_id = Customer.id
                    INNER JOIN ReservationDate ON Reservation.reservation_number = ReservationDate.reservation_number
                    INNER JOIN Object ON ReservationDate.object_number = Object.object_number
                    INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id
                WHERE Reservation.start_date = @today;
            ", (reservation, data, type, customer) => {
                data.Type = type;
                reservation.Objects.Add(data);
                reservation.CustomerData = customer;
                return reservation;
            },new { today = DateTime.Today }, splitOn:"reservation_number,object_number,id,id").GroupBy(x => x.reservation_number).Select(g => {
                Reservation groupedData = g.First();
                List<ObjectData> listObjects = g.Select(p => p.Objects.Single()).ToList();
                groupedData.Objects = listObjects.DistinctBy(a => a.object_number).ToList();
            
                return groupedData;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
    
    public List<ObjectAvailability> SelectObjectAvailabilities(DateTime? now)
    {
        DateTime StartOfWeek = now != null ? DateTime.Now.StartOfOtherWeek((DateTime) now, DayOfWeek.Monday) :  DateTime.Now.StartOfWeek(DayOfWeek.Monday);
        List<ObjectType> objectTypes = ObjectRepo.SelectObjectTypes();
        List<ObjectAvailability> availabilities = new List<ObjectAvailability>();
        
        foreach (ObjectType objectType in objectTypes) {
            List<ObjectAmount> objectAmounts = new List<ObjectAmount> { new ObjectAmount { amount = 10000, object_type_id = objectType.id } };

            for (int i = 0; i < 7; i++) {
                DateTime currentDay = StartOfWeek.AddDays(i);
                int availableAmount = SelectAvailableObjects(currentDay, currentDay, objectAmounts).Count;
                ObjectAvailability availability = new ObjectAvailability { AvailableObjectType = objectType };
                switch (i) {
                    case 0:
                        availability.MondayAvailable = availableAmount;
                        break;
                    case 1:
                        availability.TuesdayAvailable = availableAmount;
                        break;
                    case 2:
                        availability.WednesdayAvailable = availableAmount;
                        break;
                    case 3:
                        availability.ThursdayAvailable = availableAmount;
                        break;
                    case 4:
                        availability.FridayAvailable = availableAmount;
                        break;
                    case 5:
                        availability.SaturdayAvailable = availableAmount;
                        break;
                    case 6:
                        availability.SundayAvailable = availableAmount;
                        break;
                }
                availabilities.Add(availability);
            }
        }

        return availabilities.GroupBy(x => x.AvailableObjectType.id).Select(g =>
        {
            ObjectAvailability groupedData = g.First();
            List<ObjectAvailability> listAvailability = availabilities.Where(x => x.AvailableObjectType.id == groupedData.AvailableObjectType.id).ToList();

            for (int i = 0; i < 7; i++) {
                switch (i) {
                    case 0:
                        groupedData.MondayAvailable = listAvailability[i].MondayAvailable;
                        break;
                    case 1:
                        groupedData.TuesdayAvailable = listAvailability[i].TuesdayAvailable;
                        break;
                    case 2:
                        groupedData.WednesdayAvailable = listAvailability[i].WednesdayAvailable;
                        break;
                    case 3:
                        groupedData.ThursdayAvailable = listAvailability[i].ThursdayAvailable;
                        break;
                    case 4:
                        groupedData.FridayAvailable = listAvailability[i].FridayAvailable;
                        break;
                    case 5:
                        groupedData.SaturdayAvailable = listAvailability[i].SaturdayAvailable;
                        break;
                    case 6:
                        groupedData.SundayAvailable = listAvailability[i].SundayAvailable;
                        break;
                }
            }

            return groupedData;
        }).ToList();
    } 

    public List<ObjectData> SelectAvailableObjects(DateTime startDate, DateTime endDate, List<ObjectAmount> objectAmounts)
    {
        // TODO FIXxxx
        try {
            Connect();
            string query = @"
                SELECT Object.*, ObjectType.*
                FROM Object 
                    INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id 
                    LEFT JOIN ReservationDate ON Object.object_number = ReservationDate.object_number 
                    LEFT JOIN Reservation ON ReservationDate.reservation_number = Reservation.reservation_number
                WHERE NOT EXISTS (
                    SELECT dte.object_number 
                    FROM ReservationDate AS dte
                    WHERE dte.day BETWEEN @startDate AND @endDate
                ) 
                OR NOT EXISTS (
                    SELECT is_r.object_number 
                    FROM ReservationDate AS is_r 
                    WHERE is_r.object_number = Object.object_number
                )
                GROUP BY Object.object_number;
            ";
            List<ObjectData> result = Connection.Query<ObjectData, ObjectType, ObjectData>(query, (data, type) =>
            {
                data.Type = type;
                return data;
            }, new { startDate, endDate }).ToList();

            List<ObjectData> finalResult = new List<ObjectData>();
            foreach (ObjectAmount objectType in objectAmounts) {
                if (objectType.amount == 0) continue;
                List<ObjectData> res = result.Where(x => x.object_type_id == objectType.object_type_id).Take(objectType.amount).ToList(); 
                if (res.Count < objectType.amount) {
                    // TODO TOO LITTLE BIKE ERROR HANDLING
                }
                finalResult.AddRange(res);
            }
            return finalResult;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    // select available without limits
    public List<ObjectData> SelectAvailableObjects(DateTime startDate, DateTime endDate)
    {
        try {
            Connect();
            string query = @"
                SELECT Object.*, ObjectType.*
                FROM Object 
                    INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id 
                    LEFT JOIN ReservationDate ON Object.object_number = ReservationDate.object_number 
                    LEFT JOIN Reservation ON ReservationDate.reservation_number = Reservation.reservation_number
                WHERE NOT EXISTS (
                    SELECT dte.object_number 
                    FROM ReservationDate AS dte
                    WHERE dte.day BETWEEN @startDate AND @endDate
                ) 
                OR NOT EXISTS (
                    SELECT is_r.object_number 
                    FROM ReservationDate AS is_r 
                    WHERE is_r.object_number = Object.object_number
                )
                GROUP BY Object.object_number;
            ";
            return Connection.Query<ObjectData, ObjectType, ObjectData>(query, (data, type) =>
            {
                data.Type = type;
                return data;
            }, new { startDate, endDate }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
    
    public int GiveOutReservation(int reservationNumber, List<ObjectData> objectsToGiveOut)
    {
        int rowCount = 0;
        try {
            Connect();
            Reservation reservation = Connection.Query<Reservation>("SELECT reservation_number, start_date, return_date FROM Reservation WHERE reservation_number = @resId LIMIT 1;", new { resId = reservationNumber }).First();
            foreach (ObjectData obj in objectsToGiveOut) {
                for (int i = 0; i < (reservation.return_date - reservation.start_date).TotalDays; i++) {
                    rowCount = Connection.ExecuteScalar<int>("INSERT INTO ReservationDate (object_number, reservation_number, `day`) VALUES (@object_number, @reservation_number, @day)", new 
                    {
                        reservation.reservation_number,
                        obj.object_number,
                        day = reservation.start_date.AddDays(i)
                    });
                }
            }
            rowCount += Connection.ExecuteScalar<int>("DELETE FROM is_reserved_at WHERE reservation_number = @resId", new { resId = reservationNumber });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;    }

    public int UpdateReservation(List<Reservation> reservationsToUpdate)
    {
        int rowCount = 0;
        try {
            Connect();
            foreach (Reservation reservation in reservationsToUpdate) {
                rowCount += Connection.ExecuteScalar<int>("UPDATE Reservation SET Paid = @paid WHERE reservation_number = @reservationNr", new { reservation.paid, reservationNr = reservation.reservation_number });
                // todo: change objects
            }
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;
    }

    public int DeleteReservation(int reservationId)
    {
        int rowCount = 0;
        try {
            Connect();
            rowCount += Connection.ExecuteScalar<int>(@"
                DELETE FROM is_reserved_at WHERE reservation_number = @reservationNr;
                DELETE FROM ReservationDate WHERE reservation_number = @reservationNr;
                DELETE FROM Reservation WHERE reservation_number = @reservationNr;
            ", new { reservationNr = reservationId });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }

    public int DeleteReservedObject(Reservation reservation)
    {
        int rowCount = 0;
        try {
            Connect();
            rowCount += Connection.ExecuteScalar<int>("DELETE FROM ReservationDate WHERE reservation_number = @reservationNr AND object_number = @objNr;", new { reservationNr = reservation.reservation_number, objNr = reservation.Objects[0].object_number });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }
}