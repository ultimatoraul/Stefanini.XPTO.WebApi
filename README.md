# Stefanini.XPTO.WebApi
Configure connectionString da WEBAPI.
Publique a WEBAPI e acrescente no web.config do projeto WEBMVC e o app.config do WindowsForms o nome do "host + nome da aplicação no IIS"
Exemplo:
  <appSettings>
    <add key="baseURL" value="http://192.168.0.169/xptoapi/" />
  </appSettings>


Alguns comentários:
Por falta de tempo hábil, algumas validações que julgo necessário não foram implementadas, algumas outras por não ter especificação.

Acredito que deve haver regras para pessoas com perfil desativado, como não poder ter um produto associado.
Outra questão é se o mesmo produto pode ser adicionado a mesma pessoa mais de uma vez.

Optei por não utilizar Procedure e Linq, o Linq poderia me ajudar nos casos das validações que citei.
Falta de capricho no visual e nos retornos foram por falta de tempo, mas paro por aqui com minhas lamentações.

O GitHub acabou segurando as pastas packages e .vs , peço que ignore :\

Fico a disposição para contato
raulsp@ymail.com
