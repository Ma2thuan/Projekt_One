Use BanHang
-- Create database
CREATE DATABASE Project1DB;
GO

-- Use the database
USE Project1DB;
GO

Drop Database Project1DB
Drop Table users
-- Create users table (đã có)
CREATE TABLE users (
    user_id INT PRIMARY KEY,
    user_birth DATE,
    user_address NVARCHAR(255),
    user_identity INT,
    user_phone INT,
    user_name NVARCHAR(255)
);
GO
ALTER TABLE users
ADD email NVARCHAR(255) NULL DEFAULT '@example.com', -- Temporary nullable email
    password NVARCHAR(255) NULL DEFAULT 'default_password';

ALTER TABLE users
ADD role NVARCHAR(50) NOt NULL DEFAULT 'User';
GO

-- Tạo bảng SavingsAccountType (đã có)
create TABLE SavingsAccountType (
    SavingsTypeID INT PRIMARY KEY,
    AccountTypeName VARCHAR(255),
    InterestRate FLOAT,
    Term INT,
    WithdrawalDays INT,
	MinimumDeposit INT,
    IsActive BIT,
    AllowsAdditionalDeposits BIT
);
GO


-- Tạo bảng passbook với khóa ngoại
CREATE TABLE passbook (
    SavingsBookID INT PRIMARY KEY,
    user_id INT,
    OpeningDate DATE,
    InitialDepositAmount INT,
	DepositAmount INT,
    InterestRate FLOAT,
    SavingsType INT,
    IsClosed BIT,
    FOREIGN KEY (SavingsType) REFERENCES SavingsAccountType(SavingsTypeID),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO

-- Tạo bảng SavingsDeposit với khóa ngoại và thuộc tính `user_id`
CREATE TABLE SavingsDeposit (
    DepositID INT PRIMARY KEY,
    SavingsBookID INT,
    user_id INT,
    DepositAmount INT,
    InterestRate FLOAT,
    DepositDate DATE,
    FOREIGN KEY (SavingsBookID) REFERENCES passbook(SavingsBookID),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO

-- Tạo bảng WithdrawalSlip với khóa ngoại và thuộc tính `user_id`
CREATE TABLE WithdrawalSlip (
    WithdrawalID INT PRIMARY KEY,
    SavingsBookID INT,
    user_id INT,
    WithdrawalAmount INT,
    WithdrawalDate DATE,
    FOREIGN KEY (SavingsBookID) REFERENCES passbook(SavingsBookID),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO


-- Thêm dữ liệu mẫu vào bảng users
INSERT INTO users (user_id, user_birth, user_address, user_identity, user_phone, user_name)
VALUES
(1, '1990-01-01', N'123 Main St', 101, 1234567890, N'Alice Johnson'),
(2, '1985-02-15', N'456 Elm St', 102, 0987654321, N'Bob Smith'),
(3, '1978-03-10', N'789 Maple St', 103, 1122334455, N'Charlie Brown');
GO

INSERT INTO users (user_id, user_birth, user_address, user_identity, user_phone, user_name)
VALUES
(4, '2000-02-15', N'987 Aim St', 102, 0987654321, N'Aimstrong Daisy'),
(5, '2000-03-10', N'787 Wesl St', 103, 1122334455, N'Neo William');
GO

-- Dữ liệu mẫu cho bảng SavingsAccountType (đã có)
INSERT INTO SavingsAccountType (SavingsTypeID, AccountTypeName, InterestRate, Term, WithdrawalDays, IsActive, AllowsAdditionalDeposits, MinimumDeposit)
VALUES 
(1, 'No Term', 0.15, 0, 15, 1, 1, 100000),
(2, '3 Months', 0.50, 3, 90, 1, 0, 100000),
(3, '6 Months', 0.55, 6, 180, 1, 0, 100000);
GO

-- Dữ liệu mẫu cho bảng passbook
INSERT INTO passbook (SavingsBookID, user_id, OpeningDate, InitialDepositAmount, DepositAmount, InterestRate, SavingsType, IsClosed)
VALUES
(1, 1, '2024-01-01', 1000000, 1000000, 0.15, 1, 0),
(2, 2, '2024-02-15', 1000000, 1000000, 0.50, 2, 0),
(3, 3, '2024-03-10', 1000000, 1000000, 0.55, 3, 0);
GO

INSERT INTO passbook (SavingsBookID, user_id, OpeningDate, InitialDepositAmount, DepositAmount, InterestRate, SavingsType, IsClosed)
VALUES
(4, 4, '2024-01-01', 4000000, 4000000, 0.15, 1, 0),
(5, 5, '2024-02-15', 5000000, 5000000, 0.50, 2, 0);
GO

-- Dữ liệu mẫu cho bảng SavingsDeposit
INSERT INTO SavingsDeposit (DepositID, SavingsBookID, user_id, DepositAmount, InterestRate, DepositDate)
VALUES 
(4, 4, 4, 3000000, 0.15, '2024-02-01')
GO


-- Dữ liệu mẫu cho bảng WithdrawalSlip
INSERT INTO WithdrawalSlip (WithdrawalID, SavingsBookID, user_id, WithdrawalAmount, WithdrawalDate)
VALUES 
(1, 1, 1, 200000, '2024-03-05'),
(2, 2, 2, 500000, '2024-11-20');
GO

drop PROCEDURE InsertSavingsDeposit
go
-- Procedure để thêm bản ghi vào SavingsDeposit (đã sửa)
CREATE PROCEDURE InsertSavingsDeposit
    @SavingsBookID INT,
    @DepositAmount INT,
    @DepositDate DATE
AS
BEGIN
    DECLARE @InterestRate FLOAT, @SavingsType INT, @LastDepositDate DATE

    -- Lấy lãi suất và loại tiết kiệm của tài khoản
    SELECT @InterestRate = InterestRate, @SavingsType = SavingsType FROM passbook WHERE SavingsBookID = @SavingsBookID

    -- Kiểm tra loại tiết kiệm và số ngày gửi thêm tối thiểu
    IF @SavingsType = 1 -- Không kỳ hạn
    BEGIN
        IF @DepositAmount < 100000
        BEGIN
            RAISERROR('Số tiền gởi thêm tối thiểu là 100.000đ', 16, 1)
            RETURN
        END
    END
    ELSE
    BEGIN
        -- Lấy ngày gửi tiền cuối cùng
        SELECT @LastDepositDate = MAX(DepositDate) FROM SavingsDeposit WHERE SavingsBookID = @SavingsBookID

        -- Kiểm tra kỳ hạn và số tiền gởi thêm
        IF DATEADD(MONTH, @SavingsType * 3, @LastDepositDate) > @DepositDate OR @DepositAmount < 100000
        BEGIN
            RAISERROR('Chỉ nhận gởi thêm tiền khi đến kỳ hạn và số tiền gởi thêm tối thiểu là 100.000đ', 16, 1)
            RETURN
        END
    END

    -- Thêm bản ghi gửi tiền
    INSERT INTO SavingsDeposit (SavingsBookID, DepositAmount, InterestRate, DepositDate)
    VALUES (@SavingsBookID, @DepositAmount, @InterestRate, @DepositDate)
END
GO
drop PROCEDURE InsertWithdrawalSlip
go
-- Procedure để thêm bản ghi vào WithdrawalSlip (đã sửa)
CREATE PROCEDURE InsertWithdrawalSlip
    @SavingsBookID INT,
    @WithdrawalAmount INT,
    @WithdrawalDate DATE
AS
BEGIN
    DECLARE @InterestRate FLOAT, @SavingsType INT, @Balance INT, @OpeningDate DATE, @IsClosed BIT

    -- Lấy thông tin của sổ tiết kiệm
    SELECT @InterestRate=InterestRate, @SavingsType=SavingsType, @Balance=InitialDepositAmount, @OpeningDate=OpeningDate, @IsClosed=IsClosed
    FROM passbook WHERE SavingsBookID=@SavingsBookID

    -- Kiểm tra trạng thái sổ và số dư
    IF @IsClosed = 1
    BEGIN
        RAISERROR('Sổ tiết kiệm đã đóng', 16, 1)
        RETURN
    END

    -- Quy định rút tiền
    IF @SavingsType=1 -- Không kỳ hạn
    BEGIN
        IF @WithdrawalDate <= DATEADD(DAY, 15, @OpeningDate) OR @WithdrawalAmount > @Balance
        BEGIN
            RAISERROR('Chỉ được rút sau 15 ngày và không quá số dư hiện có', 16, 1)
            RETURN
        END
    END
    ELSE
    BEGIN
        IF @WithdrawalDate < DATEADD(MONTH, @SavingsType * 3, @OpeningDate) OR @WithdrawalAmount < @Balance
        BEGIN
            RAISERROR('Chỉ được rút khi quá kỳ hạn và phải rút hết toàn bộ', 16, 1)
            RETURN
        END

        -- Tính lãi suất không kỳ hạn
        SET @InterestRate = 0.15
    END

    -- Tính tiền lãi
    DECLARE @Interest INT
    SET @Interest = @Balance * @InterestRate * @SavingsType / 100

    -- Thêm bản ghi rút tiền
    INSERT INTO WITHDRAWALSLIP (SavingsBookID, WithdrawalAmount, WithdrawalDate)
    VALUES (@SavingsBookID, @WithdrawalAmount, @WithdrawalDate)

    -- Cập nhật lại trạng thái sổ tiết kiệm
    IF @WithdrawalAmount = @Balance
    BEGIN
        UPDATE passbook SET IsClosed = 1 WHERE SavingsBookID = @SavingsBookID
    END
END
GO
drop PROCEDURE InsertPassbook
go
-- Procedure để thêm bản ghi vào passbook (đã sửa)
CREATE PROCEDURE InsertPassbook
    @SavingsBookID INT,
    @user_id INT,
    @OpeningDate DATE,
    @InitialDepositAmount INT,
    @SavingsType INT
AS
BEGIN
    IF @InitialDepositAmount < 1000000
    BEGIN
        RAISERROR('Số tiền gửi tối thiểu để mở sổ là 1.000.000đ', 16, 1)
        RETURN
    END

    DECLARE @InterestRate FLOAT

    -- Lấy lãi suất ứng với loại tiết kiệm
    SELECT @InterestRate = InterestRate FROM SavingsAccountType WHERE SavingsTypeID = @SavingsType

    -- Thêm bản ghi vào bảng passbook
    INSERT INTO passbook (SavingsBookID, user_id, OpeningDate, InitialDepositAmount, InterestRate, SavingsType, IsClosed)
    VALUES (@SavingsBookID, @user_id, @OpeningDate, @InitialDepositAmount, @InterestRate, @SavingsType, 0)
END
GO


drop trigger AddDeposit
-- Trigger cập nhật tiền gửi sau khi tạo mới SavingsDeposit
CREATE TRIGGER AddDeposit
ON SavingsDeposit
AFTER INSERT
AS
BEGIN
    DECLARE @SavingsBookID INT;
    DECLARE @DepositAmount FLOAT;
    DECLARE @CurrentDepositAmount FLOAT;

    -- Lấy dữ liệu từ bản ghi vừa được chèn
    SELECT
        @SavingsBookID = SavingsBookID,
        @DepositAmount = DepositAmount
    FROM inserted;

    -- Lấy giá trị tiền gửi hiện tại
    SELECT
        @CurrentDepositAmount = DepositAmount
    FROM passbook
    WHERE SavingsBookID = @SavingsBookID;

    -- Cập nhật tổng tiền gửi mới vào passbook
    UPDATE passbook
    SET DepositAmount = @CurrentDepositAmount + @DepositAmount
    WHERE SavingsBookID = @SavingsBookID;
END;
GO

drop TRIGGER SubtractWithdrawal
go
-- Trigger cập nhật tiền gửi sau khi tạo mới WithdrawalSlip
CREATE TRIGGER SubtractWithdrawal
ON WithdrawalSlip
AFTER INSERT
AS
BEGIN
    DECLARE @SavingsBookID INT;
    DECLARE @WithdrawalAmount FLOAT;
    DECLARE @user_id INT;
    DECLARE @CurrentDate DATE = GETDATE();

    -- Lấy dữ liệu từ bản ghi vừa được chèn
    SELECT
        @SavingsBookID = SavingsBookID,
        @WithdrawalAmount = WithdrawalAmount,
        @user_id = user_id
    FROM inserted;

    DECLARE @PassbookSavingsType INT = (SELECT SavingsType FROM passbook WHERE SavingsBookID = @SavingsBookID);
    DECLARE @PassbookInitialDepositAmount FLOAT = (SELECT InitialDepositAmount FROM passbook WHERE SavingsBookID = @SavingsBookID);
    DECLARE @PassbookDepositAmount FLOAT;
    DECLARE @OpeningDate DATE = (SELECT OpeningDate FROM passbook WHERE SavingsBookID = @SavingsBookID);

    -- Kiểm tra loại tiết kiệm không kỳ hạn
    IF @PassbookSavingsType = 1 
    BEGIN
        DECLARE @DaysDiff INT = DATEDIFF(DAY, @OpeningDate, @CurrentDate);
        
        IF @DaysDiff > 15 
        BEGIN
            -- Rút tiền thông thường nếu còn đủ số dư
            IF @WithdrawalAmount <= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = DepositAmount - @WithdrawalAmount
                WHERE SavingsBookID = @SavingsBookID;
            END
            ELSE
            -- Rút toàn bộ số tiền nếu số dư nhỏ hơn số tiền yêu cầu
            IF @WithdrawalAmount >= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = 0, IsClosed = 1 -- Đóng sổ
                WHERE SavingsBookID = @SavingsBookID;
            END
        END
    END
    ELSE
    BEGIN
        DECLARE @MonthsDiff INT = DATEDIFF(MONTH, @OpeningDate, @CurrentDate);
        
        -- Kiểm tra loại tiết kiệm kỳ hạn 3 tháng
        IF @PassbookSavingsType = 2 AND @MonthsDiff >= 3 
        BEGIN
            DECLARE @Interest3 FLOAT = @PassbookInitialDepositAmount * 0.50 * 3;
            SET @PassbookDepositAmount = (SELECT DepositAmount + @Interest3 FROM passbook WHERE SavingsBookID = @SavingsBookID);
            DECLARE @RemainingMonths3 INT = @MonthsDiff - 3;
            
            IF @RemainingMonths3 > 0
            BEGIN
                SET @PassbookDepositAmount = @PassbookDepositAmount + (@PassbookDepositAmount * 0.15 * @RemainingMonths3);
            END

            -- Kiểm tra số tiền yêu cầu rút
            IF @WithdrawalAmount <= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = DepositAmount - @WithdrawalAmount
                WHERE SavingsBookID = @SavingsBookID;
            END
            ELSE
            -- Rút toàn bộ số tiền
            IF @WithdrawalAmount >= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = 0, IsClosed = 1
                WHERE SavingsBookID = @SavingsBookID;
            END
        END
        
        -- Kiểm tra loại tiết kiệm kỳ hạn 6 tháng
        IF @PassbookSavingsType = 3 AND @MonthsDiff >= 6 
        BEGIN
            DECLARE @Interest6 FLOAT = @PassbookInitialDepositAmount * 0.55 * 6;
            SET @PassbookDepositAmount = (SELECT DepositAmount + @Interest6 FROM passbook WHERE SavingsBookID = @SavingsBookID);
            DECLARE @RemainingMonths6 INT = @MonthsDiff - 6;
            
            IF @RemainingMonths6 > 0
            BEGIN
                SET @PassbookDepositAmount = @PassbookDepositAmount + (@PassbookDepositAmount * 0.15 * @RemainingMonths6);
            END

            -- Kiểm tra số tiền yêu cầu rút
            IF @WithdrawalAmount <= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = DepositAmount - @WithdrawalAmount
                WHERE SavingsBookID = @SavingsBookID;
            END
            ELSE
            -- Rút toàn bộ số tiền
            IF @WithdrawalAmount >= @PassbookDepositAmount 
            BEGIN
                UPDATE passbook
                SET DepositAmount = 0, IsClosed = 1
                WHERE SavingsBookID = @SavingsBookID;
            END
        END
    END
END;
GO



drop TRIGGER CalculateInterest
go 
-- Trigger này sẽ tự động tính lãi sau mỗi lần cập nhật lãi suất tiết kiệm theo từng loại
CREATE TRIGGER CalculateInterest
ON SavingsDeposit
AFTER INSERT
AS
BEGIN
    DECLARE @SavingsBookID INT;
    DECLARE @DepositAmount FLOAT;
    DECLARE @InterestRate FLOAT;
    DECLARE @DepositDate DATE;
    DECLARE @CurrentDate DATE = GETDATE();

    -- Lấy dữ liệu từ bản ghi vừa được chèn
    SELECT 
        @SavingsBookID = SavingsBookID,
        @DepositAmount = DepositAmount,
        @InterestRate = InterestRate,
        @DepositDate = DepositDate
    FROM inserted;

    -- Tính số tháng giãn cách giữa hiện tại và ngày gửi tiền
    DECLARE @MonthsDiff INT = DATEDIFF(MONTH, @DepositDate, @CurrentDate);

    -- Lấy thông tin sổ tiết kiệm
    DECLARE @PassbookSavingsType INT = (SELECT SavingsType FROM passbook WHERE SavingsBookID = @SavingsBookID);
    DECLARE @PassbookDepositAmount FLOAT = (SELECT DepositAmount FROM passbook WHERE SavingsBookID = @SavingsBookID);
    DECLARE @InterestSummary FLOAT = 0;

    -- Tính tiền lãi cho từng kỳ hạn
    IF @PassbookSavingsType = 1 -- Không kỳ hạn
    BEGIN
        IF @MonthsDiff > 0
        BEGIN
            SET @InterestSummary = @DepositAmount * 0.15 * @MonthsDiff;
        END
    END
    ELSE IF @PassbookSavingsType = 2 -- Kỳ hạn 3 tháng
    BEGIN
        IF @MonthsDiff >= 3
        BEGIN
            SET @InterestSummary = @DepositAmount * 0.50 * 3;
            DECLARE @RemainingMonths3 INT = @MonthsDiff - 3;

            IF @RemainingMonths3 > 0
            BEGIN
                SET @InterestSummary = @InterestSummary + @DepositAmount * 0.15 * @RemainingMonths3;
            END
        END
    END
    ELSE IF @PassbookSavingsType = 3 -- Kỳ hạn 6 tháng
    BEGIN
        IF @MonthsDiff >= 6
        BEGIN
            SET @InterestSummary = @DepositAmount * 0.55 * 6;
            DECLARE @RemainingMonths6 INT = @MonthsDiff - 6;

            IF @RemainingMonths6 > 0
            BEGIN
                SET @InterestSummary = @InterestSummary + @DepositAmount * 0.15 * @RemainingMonths6;
            END
        END
    END

    -- Cập nhật tiền gửi sau khi tính lãi
    UPDATE passbook
    SET DepositAmount = @DepositAmount + @InterestSummary
    WHERE SavingsBookID = @SavingsBookID;
END;
GO

