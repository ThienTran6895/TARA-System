var golbal = golbal || {};
(function ()
{
    //privite properties and method
    var ns_core = "core";
    //function ig(i) { }
    
    //publish properties

    golbal.extendns = function(ns, ns_string)
    {
        var parts = ns_string.split('.'), parent = ns, pl, i;

        if (parts[0] == ns_core)
            parts.slice(1);

        pl = parts.length;
        for (i = 0; i < pl; i++)
        {
            //create property if dosent exit
            if (typeof parent[i] == 'undefined')
                parent[parts[i]] = {};
            parent = parent[parts[i]];
        }
        return parent;
    }

    golbal.extend = function extend(destination, source)
    {
        var toString = Object.prototype.toString, obj = toString.call({});
        for (var p in source)
        {
            if (source[p] && obj == toString.call(source[p]))
            {
                destination[p] = destination[p] || {};
                extend(destination[p], source[p]);
            } else
            {
                destination[p] = source[p];
            }
        }
        return destination
    }
    
}).apply(golbal);
