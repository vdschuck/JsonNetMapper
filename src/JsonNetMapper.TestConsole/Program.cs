// See https://aka.ms/new-console-template for more information

using JsonNetMapper;

var configJson = @"{
    'stores': {
        'physical': {            
            'selector': 'Stores',
        },
        'virtual': {
            'selector': 'Virtual',
            'type': 'string',
            'format': '',
        }
    },
    'title': {
        'selector': 'Manufacturers[0].Name',
        'type': 'string',
        'format': '',
    },
    'products': {
        'selector': 'Manufacturers[1].Products',
        'type': 'object',
        'format': '',
    },
    'value': {
        'selector': 'Manufacturers[0].Products[0].Price',
        'type': 'decimal',
        'format': '{0:0,0.00}',
    },
    'date': {
        'selector': 'Manufacturers[0].Date',
        'type': 'date',
        'format': 'dd/MM/yyyy',
    }
}";

var sourceJson = @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Date': '2022-05-12',
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil',
          'Price': 5015.11
        }
      ]
    },
    {
      'Date': '2022-05-30',
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
jsonNetMapper.BuildNewJson();

Console.WriteLine(jsonNetMapper.Response);
