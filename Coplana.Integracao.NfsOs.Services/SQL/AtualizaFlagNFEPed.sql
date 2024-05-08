UPDATE OWTR T 
SET T."U_ImportNFE" = 'S' 
FROM OWTR T
INNER JOIN WTR1 T1 ON T."DocEntry" = T1."DocEntry" 
WHERE T1."BaseRef" = '{0}'