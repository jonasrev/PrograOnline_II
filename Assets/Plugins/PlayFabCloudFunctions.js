// args es el paramaqtro que contiene la data que viene desde unity
handlers.setUserData = function(args,context)
{
       var data = (); //en este diccionario guardamos la data que viene de unity

       data[args,keys] = args.value.toString();
// aqui se guardan todas la llaves en el diccionario con su respectivo valor
// hace lo mismo que el for en unity.

       // esta es una funcion del palyfab
       server.updateUserData(
       {
              PlayFabID: currentPlayerID
       });
       return{
              Success;true
              ];

};