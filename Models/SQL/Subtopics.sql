USE [learning]
GO

/****** Object:  Table [dbo].[Subtopics]    Script Date: 11/11/2025 15:08:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Subtopics](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Topic_ID] [int] NULL,
	[Description] [nchar](10) NULL,
	[Logged_Date] [date] NULL,
	[Estimated_hours] [int] NULL,
	[Pluralsight] [bit] NULL,
	[Long_description] [text] NULL,
	[Completed] [bit] NULL,
	[Priority] [int] NULL,
 CONSTRAINT [PK_Subtopics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Subtopics]  WITH CHECK ADD  CONSTRAINT [FK_Subtopics_Topics] FOREIGN KEY([Topic_ID])
REFERENCES [dbo].[Topics] ([ID])
GO

ALTER TABLE [dbo].[Subtopics] CHECK CONSTRAINT [FK_Subtopics_Topics]
GO


