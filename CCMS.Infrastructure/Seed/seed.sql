-- ==========================================================
-- CCMS Authentication & Authorization Seed Script
-- Target Table: Users (Id INT IDENTITY, Username NVARCHAR, PasswordHash NVARCHAR, Role NVARCHAR)
-- ==========================================================

-- Seed Data for Court Officer (Username: court1, Password: Password123, Role: Court)
INSERT INTO [Users] ([Username], [PasswordHash], [Role]) 
VALUES ('court1', '$2a$11$fEhPFlUzrbPbqgVv7Gv9FOJWnJMl7CLR402AahenD9W23/hwHFI7e', 'Court');

-- Seed Data for Bank Officer (Username: bank1, Password: Password123, Role: Bank)
INSERT INTO [Users] ([Username], [PasswordHash], [Role]) 
VALUES ('bank1', '$2a$11$fEhPFlUzrbPbqgVv7Gv9FOJWnJMl7CLR402AahenD9W23/hwHFI7e', 'Bank');
