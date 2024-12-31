
--ALEX GUEVARA MORE

USE TIENDATECNOLOGIA
GO

--PROC PARA LISTAR TODOS LOS ARTICULOS
--------------------------------
CREATE OR ALTER PROC PA_LISTAR_ARTICULOS
AS
SELECT A.cod_art, nom_art,
        A.pre_art,stk_art
FROM Articulos A
	WHERE A.stk_art>=0 
	
GO
---------------------------
exec PA_LISTAR_ARTICULOS  
go
---------------------------


--PROC PARA REALIZAR BUSQUEDA POR NOMBRE
-------------------------------------
CREATE OR ALTER PROC FILTRAR_PROD
@NOMBRE varchar(20)
AS
SELECT A.cod_art, nom_art,
        A.pre_art,stk_art
FROM Articulos A
	WHERE A.stk_art>=0 
	AND  nom_art LIKE '%' + @NOMBRE + '%'
GO
----------------------------------------
exec FILTRAR_PROD  @NOMBRE = 'MON'
go
----------------------------------------

-- constraint de tipo check sobre la columna 
-- stk_prod de la tabla ARTICULOS que no permite 
-- tener un stock negativo

ALTER TABLE Articulos
	ADD CONSTRAINT CK_STK_PROD CHECK(STK_ART>=0)
GO




