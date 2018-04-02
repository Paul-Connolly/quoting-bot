ALTER TABLE ErrorLog
DROP COLUMN Error;

ALTER TABLE ErrorLog
ADD ErrorMessage nvarchar(MAX);