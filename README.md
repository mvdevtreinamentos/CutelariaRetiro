# CutelariaRetiro
Software Deskto para controle de Cutelarias que prestam serviço de amolagem de ferramentas e venda de materiais

*** LEIA COM ATENÇÃO E PRESTE ATENÇÃO NA FODA ***

Autor: Marcos Vinícius P. Oliveira
Email: mvdevtreinamentos@gmail.com

Este projeto é uma doação a comunidade, uma ação
que eu julgo benéfica para os iniciantes em programação
Sintam-se a vontade para modificar o projeto como bem entenderem
Podem tambem compilar o projeto para fins comerciais (Sim, eu
AUTORIZO o uso deste projeto para ganhar dinheiro $$$, caso queiram)

Não me responsabilizo por bugs, inconsistencias ou falta de recursos
presentes no código até esta data (30/11/2018 10:00:00)

------------------------------------------------------------------------
Este projeto é desenvolvido sobre plataforma Microsoft, portanto,
compatível com Windows.
Ele também podem funcionar em Linux através do Mono ou emulando através
do Wine (tambem não me responsabilizo por isso nem irei ajudar a faze-lo, pois
existem diversos tutoriais no YouTube ensinando o mesmo)

A arquitertura do programa está configurada para "Any CPU", ou seja,
é capaz de rodar perfeitamente em processador x86 ou x64 
(ou seja, funciona tanto em 32 quant 64 bits)

Os recursos utilizados no programa são: 
	
	* EntityFramework - ORM de Persistencia de dados
	* WPF - Windows Presentation Foudation, para criação das janelas (Views)
	* Padrão de projeto Repository (Repositório de dados Genérico)
	* Padrão DAL e BLL (Data Access Layer, e Business Logic's Layer)
	* DBFirst usando DataModel .EDMX

--------------------------------------------------------------------------

Instruções de compilação:
	* Possuir instalado o Microsoft .NET Framework v4.5.1
	* Possuir instalado o Microsoft SQL Server Compact Edition 4.x
	* Possuir instalado o Microsoft Visual Studio 2015 em diante (Community/Professional/Enterprise)

IMPORTANTE: Assim que aberto o projeto no Visual Studio, deve ser feita a primeira compilação
do projeto (podendo ser atraves do botão "Iniciar", ou "Run" em ingles)
Nesse momento, o programa irá criar o banco de dados em arquivo do SQL CE em
C:\Cutelaria Retiro\Data\CutelariaRetiro.sdf

Apartir desta primeira compilação, o projeto estará pronto para ser customizado
---------------------------------------------------------------------------


Faça bom proveito! :D
