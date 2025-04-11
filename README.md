Sistema de Gestão Automotiva
Este sistema foi desenvolvido para gerenciar as atividades de uma oficina automotiva, incluindo o controle de veículos, funcionários, serviços e etapas de trabalho. Através da interface web, é possível cadastrar, editar e consultar dados relacionados aos carros, serviços realizados e funcionários da oficina.

Tecnologias Utilizadas
O sistema foi desenvolvido com as seguintes tecnologias:

ASP.NET Core: Framework para desenvolvimento web back-end, utilizado para criar a aplicação.

Entity Framework Core: ORM (Object-Relational Mapper) utilizado para gerenciar as interações com o banco de dados.

SQL Server: Banco de dados relacional para armazenar as informações da aplicação.

Bootstrap: Framework CSS para design responsivo e moderno da interface de usuário.

HTML/CSS/JavaScript: Tecnologias de front-end para criar a interface da aplicação e interatividade.

jQuery: Biblioteca JavaScript para facilitar a manipulação do DOM e interações na página.

Funcionalidades
Cadastro de Carros: Registra carros com informações como modelo, cor, cliente associado, etc.

Cadastro de Funcionários: Permite cadastrar e editar informações de funcionários, incluindo especialidade e status.

Cadastro de Serviços: Gerencia os serviços que são realizados na oficina, associando-os aos carros e funcionários.

Gestão de Etapas: Acompanhe as etapas do serviço realizado nos carros, como "Em andamento", "Finalizado", etc.

Interface Responsiva: O sistema é projetado para funcionar tanto em desktops quanto dispositivos móveis.

Instalação e Configuração
Para rodar a aplicação em sua máquina local, siga os passos abaixo:

1. Pré-requisitos
Antes de rodar o sistema, você precisa garantir que tem as seguintes ferramentas instaladas:

.NET SDK: Certifique-se de que você tem o .NET SDK instalado em sua máquina. Download aqui.

SQL Server: O sistema requer um banco de dados SQL Server para armazenar as informações. Você pode usar o SQL Server Express para uma instalação simples.

Visual Studio Code ou Visual Studio: Editor de código para desenvolvimento (Visual Studio recomendado para .NET). 


2. Clonar o Repositório
Clone este repositório na sua máquina local: 

3. Configurar o Banco de Dados
Crie um banco de dados no SQL Server e configure a conexão no arquivo appsettings.json:

json
Copiar
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GestaoAutomotivaDB;Trusted_Connection=True;" 

Se você estiver utilizando o SQL Server Express, altere o servidor para:

json
Copiar
"Server=localhost\\SQLEXPRESS;Database=GestaoAutomotivaDB;Trusted_Connection=True;"
  }
}
4. Migrar o Banco de Dados
Execute o comando para gerar e aplicar as migrações do banco de dados:

bash
Copiar
dotnet ef database update
5. Executar a Aplicação
Com a configuração do banco de dados concluída, execute a aplicação com o comando:

bash
Copiar
dotnet run
A aplicação estará disponível no endereço http://localhost:5000.

6. Publicação
Caso deseje publicar a aplicação, use o comando abaixo para criar uma versão auto-contida, com todas as dependências inclusas:

bash
Copiar
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
O arquivo será gerado na pasta bin\Release\netcoreapp3.1\win-x64\publish.








