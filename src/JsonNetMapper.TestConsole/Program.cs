// See https://aka.ms/new-console-template for more information

using JsonNetMapper;

var configJson = @"{
    'stores': {
        'physical': {            
            'selector': 'Stores'
        },
        'virtual': {
            'selector': 'Virtual'
        }
    },
    'title': {
        'selector': 'Manufacturers[0].Name'
    },
    'products': {
        'selector': 'Manufacturers[1].Products'
    },
}";

var sourceJson = @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}";

var jsonNetMapper = new JsonMapper(configJson, sourceJson);
var response = jsonNetMapper.BuildNewJson();

Console.WriteLine(response);
