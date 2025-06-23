--Database Creation
use master
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EventEase1')
DROP DATABASE EventEase1
CREATE DATABASE EventEase1

USE EventEase1

--TABLE CREATION
CREATE TABLE Venue (
VenueID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
VenueName VARCHAR(250) NOT NULL,
Locations VARCHAR(250) NOT NULL,
Capacity INT NOT NULL,
ImageUrl VARCHAR(MAX) NOT NULL,

);

CREATE TABLE Events (
    EventID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    EventName VARCHAR(250) NOT NULL,
    EventDate DATE NOT NULL,
    Descriptions VARCHAR(250) NOT NULL, -- Fixed typo here
    VenueID INT
);

--we are going to change existing the table events 
Alter table Events
ADD FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE;

--TABLE CREATION
CREATE TABLE Booking (
BookingID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
EventID INT,
VenueID INT,
BookingDate date

);

Alter table Booking
ADD FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE;

--Altered table
Alter table Booking
ADD FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE NO ACTION;


-- Insert a venue
INSERT INTO Venue (VenueName, Locations, Capacity, ImageUrl)
VALUES ('Grand Hall', '123 Main Street, NY', 500, 'https://www.google.com/imgres?q=kryptonite&imgurl=https%3A%2F%2Fmiro.medium.com%2Fv2%2Fresize%3Afit%3A2000%2F1*n5KDCA_vv8LZ_Ykp5PIJGQ.jpeg&imgrefurl=https%3A%2F%2Febonstorm.medium.com%2Fis-kryptonite-made-from-the-element-krypton-4303d237208b&docid=gtulH4mHVUX6BM&tbnid=sp_i8Kh9OrrDqM&vet=12ahUKEwj9iu_CjcSMAxVLhP0HHVPRLUEQM3oECHYQAA..i&w=1538&h=855&hcb=2&ved=2ahUKEwj9iu_CjcSMAxVLhP0HHVPRLUEQM3oECHYQAA'); 

-- Insert an event linked to the correct venue
INSERT INTO Events (EventName, EventDate, Descriptions, VenueID)
VALUES ('Tech Conference 2025', '2025-06-15', 'Annual technology conference.', 1);

-- Insert a booking linked to the correct event and venue
INSERT INTO Booking (EventID, VenueID, BookingDate)
VALUES (1, 1, GETDATE());



SELECT * FROM Venue;
SELECT * FROM Events;
SELECT * FROM Booking;
