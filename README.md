Microserviço de Usuários em .NET
================================

Este projeto é um microserviço de gerenciamento de usuários em .NET, construído para atender aos requisitos de um teste técnico. O serviço expõe uma API RESTful para operações de CRUD de usuários, autenticação com JWT e troca de senha.

A solução foi projetada seguindo os princípios da **Clean Architecture**, garantindo uma separação clara de responsabilidades, alta coesão e baixo acoplamento entre as camadas de negócio, aplicação e infraestrutura.

Tecnologias Utilizadas
----------------------

*   **.NET 9.0:** Framework principal para o desenvolvimento da aplicação.
    
*   **ASP.NET Core:** Para a construção da API RESTful.
    
*   **Entity Framework Core:** ORM para o acesso ao banco de dados relacional.
    
*   **SQL Server:** Banco de dados relacional para persistência de dados.
    
*   **Docker:** Para o empacotamento da aplicação e do banco de dados em containers.
    
*   **Serilog:** Biblioteca de logging estruturado para observabilidade.
    
*   **xUnit & Moq:** Framework de testes e biblioteca de mocking para testes unitários.
    
*   **JWT (JSON Web Token):** Para a autenticação e autorização seguras.
    

Arquitetura do Projeto
----------------------

A solução é organizada em quatro projetos, seguindo o padrão Clean Architecture:

*   **MicroservicoUsuarios.Core:** Contém as entidades de domínio (Usuario) e as interfaces de repositório. Esta é a camada mais interna e não possui dependências de outros projetos.
    
*   **MicroservicoUsuarios.Application:** Contém a lógica de negócio da aplicação (serviços) e os DTOs.
    
*   **MicroservicoUsuarios.Infrastructure:** Contém a implementação do repositório (IUsuarioRepository) usando o Entity Framework Core e a configuração do banco de dados.
    
*   **MicroservicoUsuarios.API:** O ponto de entrada da aplicação, onde os endpoints são expostos e a injeção de dependência é configurada.
    

Pré-requisitos
--------------

Para rodar este projeto, você precisará ter o **.NET SDK 9.0** e o **Docker** instalados em sua máquina.

Como Rodar a Aplicação
----------------------

A forma mais fácil de executar a aplicação é usando o docker-compose. A partir da raiz do projeto, execute o seguinte comando no terminal:

```bash
docker-compose up --build
```

Este comando irá:

1.  Construir a imagem da aplicação .NET.
    
2.  Baixar e iniciar o container do SQL Server.
    
3.  Aplicar as migrações do banco de dados automaticamente.
    
4.  Iniciar o microserviço na porta 8080.
    

A API estará disponível em: http://localhost:8080

## Endpoints da API

A API está protegida com JWT. O endpoint de criação de usuário é público, mas os demais requerem autenticação.

| Método | URL                              | Descrição                                  |
|--------|---------------------------------|--------------------------------------------|
| POST   | `/api/usuarios`                  | Cria um novo usuário. (Público)             |
| POST   | `/api/auth/login`                | Autentica um usuário e retorna um token JWT. (Público) |
| GET    | `/api/usuarios`                  | Lista todos os usuários.        |
| GET    | `/api/usuarios/{id}`             | Busca um usuário por ID.        |
| PUT    | `/api/usuarios/{id}`             | Atualiza um usuário.             |
| DELETE | `/api/usuarios/{id}`             | Exclui um usuário.               |
| POST   | `/api/auth/change-password/{id}`| Altera a senha do usuário.      |


Testes e Cobertura de Código
----------------------------

Os testes unitários foram escritos usando **xUnit** e **Moq**, com uma cobertura de código de **85%** na lógica de negócio.

Para rodar os testes e gerar o relatório de cobertura, use os seguintes comandos na raiz do projeto:

1.  Rodar os testes e coletar os dados de cobertura:
    ```bash
    dotnet test --collect:"XPlat Code Coverage"
    ```
    
2.  Gerar o relatório visual em HTML:
    ```bash
    reportgenerator -reports:tests/MicroservicoUsuarios.Tests.Unit/TestResults/*/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
    ```
    

Após a execução do segundo comando, o relatório estará disponível no arquivo coveragereport/index.html.
