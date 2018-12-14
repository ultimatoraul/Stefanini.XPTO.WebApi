# Stefanini.XPTO.WebApi
Configure connectionString da WEBAPI.<br />
Publique a WEBAPI e adicione no web.config do projeto WEBMVC e o app.config do WindowsForms um "appSettings" <br />
Com a key="baseURL" e com o value="host + nome da aplicação no IIS", por exemplo, "http://192.168.0.169/xptoapi/" 
<br />

<b>Alguns comentários:</b><br />
Por falta de tempo hábil, algumas validações que julgo necessário não foram implementadas, algumas outras por não ter especificação.
<br />

Acredito que deve haver regras para pessoas com perfil desativado, como não poder ter um produto associado.<br />
Outra questão é se o mesmo produto pode ser adicionado a mesma pessoa mais de uma vez.
<br />

Optei por não utilizar Procedure e Linq, o Linq poderia me ajudar nos casos das validações que citei.<br />
Falta de capricho no visual e nos retornos foram por falta de tempo, mas paro por aqui com minhas lamentações.
<br />

O GitHub acabou segurando as pastas packages e .vs , peço que ignore :\

Fico a disposição para contato
raulsp@ymail.com
