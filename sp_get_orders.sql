USE SalesOrderDB;
GO

ALTER PROCEDURE sp_get_orders
    @Keyword VARCHAR(100) = NULL,
    @OrderDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        so.SALES_SO_ID AS SalesSoId,
        so.SO_NO AS SoNo,
        so.ORDER_DATE AS OrderDate,
        so.COM_CUSTOMER_ID AS CustomerId,
        c.CUSTOMER_NAME AS CustomerName,
        so.ADDRESS AS Address,
        CAST(ISNULL(SUM(i.QUANTITY * i.PRICE), 0) AS DECIMAL(18,2)) AS GrandTotal,
        COUNT(*) OVER() AS TotalCount
    FROM 
        SALES_SO so
    INNER JOIN 
        COM_CUSTOMER c ON so.COM_CUSTOMER_ID = c.COM_CUSTOMER_ID
    LEFT JOIN 
        SALES_SO_LITEM i ON so.SALES_SO_ID = i.SALES_SO_ID
    WHERE 
        (@Keyword IS NULL OR so.SO_NO LIKE '%' + @Keyword + '%' OR c.CUSTOMER_NAME LIKE '%' + @Keyword + '%')
        AND (@OrderDate IS NULL OR CAST(so.ORDER_DATE AS DATE) = @OrderDate)
    GROUP BY 
        so.SALES_SO_ID, so.SO_NO, so.ORDER_DATE, so.COM_CUSTOMER_ID, c.CUSTOMER_NAME, so.ADDRESS
    ORDER BY 
        so.ORDER_DATE DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO