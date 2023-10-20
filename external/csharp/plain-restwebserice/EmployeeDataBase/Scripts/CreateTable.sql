USE [Company]
GO

/****** Object:  Table [dbo].[Employee]    Script Date: 09/10/2010 12:51:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employee](
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[ID] [int] NOT NULL,
	[Designation] [nvarchar](50) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

Insert into Employee (FirstName, LastName, ID, Designation)
Values ('Anshu', 'Dutta', 3550, 'SSE')

Insert into Employee (FirstName, LastName, ID, Designation)
Values ('John', 'Doe', 3551, 'SE')

Insert into Employee (FirstName, LastName, ID, Designation)
Values ('Jane', 'Doe', 3552, 'Manager')