--Creation Script for Student-Assignment ERD
--Nathan Davis

CREATE DATABASE A3_Davis

ON PRIMARY
(
NAME='A3_Davis',
FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\A3_Davis.mdf',
--FILENAME LOCAL
SIZE = 12MB,
MAXSIZE = 50MB,
FILEGROWTH = 18%
)

LOG ON

(
NAME='A3_Davis_Log',
FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\A3_Davis_Log.ldf',
--FILENAME LOCAL
SIZE = 12MB,
MAXSIZE = 50MB,
FILEGROWTH = 10%
);

GO

USE A3_Davis

CREATE TABLE Student
(
StudentId		int			NOT NULL	IDENTITY(5000,1),
FName			TEXT		NOT NULL,
LName			TEXT		NOT NULL,
Email			varchar(50)	NOT NULL,
Dept_no			varchar(4)
)
CREATE TABLE Assignment
(
AssignmentId		int				NOT NULL	IDENTITY,
AssignmentName		TEXT,
AssignmentDesc		TEXT,
AssignmentDueDate	SMALLDATETIME	NOT NULL,
MaxPossibleGrade	int				NOT NULL,
SubmissionType		varchar(20)		NOT NULL
)
CREATE TABLE Submits
(
StudentId		int				NOT NULL,
AssignmentId	int				NOT NULL,
SubmissionDate	SMALLDATETIME	NOT NULL,
SubmissionGrade	int				NOT NULL
)

ALTER TABLE Student
	ADD CONSTRAINT PK_StudentId
	Primary Key (StudentId)

ALTER TABLE ASSIGNMENT
	ADD CONSTRAINT PK_AssignmentId
	Primary Key (AssignmentId)

ALTER TABLE Submits
	ADD CONSTRAINT PK_StudentId_AssignmentId
	Primary Key (StudentId, AssignmentId)

--Add FK's to Submits

ALTER TABLE Submits
	ADD 
	
	CONSTRAINT FK_StudentId
	FOREIGN KEY (StudentId) REFERENCES Student (StudentId)
	ON UPDATE CASCADE
	ON DELETE CASCADE,

	CONSTRAINT FK_AssignmentId
	FOREIGN KEY (AssignmentId) REFERENCES Assignment (AssignmentId)
	ON UPDATE CASCADE
	ON DELETE CASCADE

GO

ALTER TABLE Assignment
	ADD

	CONSTRAINT CK_AssignmentSubmissionType
	CHECK (SubmissionType IN ('Text Entry', 'File Upload', 'Media Recording', 'Website URL')),

	CONSTRAINT DK_AssignmentNameDefault
	DEFAULT 'CS 3550 Assignment' FOR AssignmentName,

	CONSTRAINT CK_MaxPossibleGradeBetween0and200
	CHECK (MaxPossibleGrade BETWEEN 0 AND 200)

ALTER TABLE Student
	ADD

	CONSTRAINT UniqueEmailPerStudent
	UNIQUE(Email),
	
	CONSTRAINT CK_ValidDept
	CHECK (Dept_no IN ('CS', 'EE', 'PH', 'LIT', 'ENG', 'MATH'))

ALTER TABLE Submits
	ADD

	CONSTRAINT CK_GradeBetween0and200
	CHECK (SubmissionGrade BETWEEN 0 AND 200)


--BULK INSERT Student FROM 'C:\Users\davis\Desktop\CS3550\Student Bulk Insert.csv' WITH (FIELDTERMINATOR=',',FIRSTROW=1) Data inserted successfully
--BULK INSERT Submits FROM 'C:\Users\davis\Desktop\CS3550\Submits Bulk Insert.csv' WITH (FIELDTERMINATOR=',',FIRSTROW=1) Data inserted successfully

--StudentId's start at 5000 and increment by 1
--Emails must be Unique Checked
INSERT INTO Student VALUES
('Nathan', 'Davis', 'nathandavis8@mail.weber.edu', 'CS'),
('Josh', 'Peck', 'joshpeck4@mail.weber.edu', 'MATH'),
('Jane', 'Doe', 'janedoe@mail.weber.edu', 'LIT')

--Testing Default Constraint on Assignment Name
INSERT INTO Assignment(AssignmentDesc, AssignmentDueDate, MaxPossibleGrade, SubmissionType)  
VALUES
('This is an assignment', '12/12/2022 23:50', 200, 'File Upload'),
(NULL, '12/30/2022 12:30', 150, 'Text Entry')

--Manually Entering Assignment Name
--Max Possible Grade must be between 0-200 Checked
--Submission Type must be within the accepted values checked
INSERT INTO Assignment VALUES
('Assignment 3', NULL, '1/1/2023 23:59', 200, 'Text Entry')

--Foreign Keys must reference value in primary tables checked
INSERT INTO Submits VALUES
(5000, 1, '12/11/2022 23:50', 150),
(5000, 2, '12/20/2022 23:50', 150),
(5001, 3, '1/1/2023 12:30', 100)

GO
PRINT 'Here are the Students'
PRINT ' '
SELECT * FROM Student

PRINT 'Here are the assignments'
PRINT ' '
SELECT * FROM Assignment

PRINT 'Here is the Submits Table'
PRINT ' '
SELECT * FROM Submits

USE Master;

GO

IF EXISTS (SELECT * FROM sysdatabases WHERE name='A3_Davis')
	DROP DATABASE A3_Davis;
