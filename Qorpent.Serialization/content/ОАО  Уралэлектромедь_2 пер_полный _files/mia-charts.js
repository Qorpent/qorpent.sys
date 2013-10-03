(function(){
    window.mia = window.mia || {};
    window.mia.charts = window.window.mia.charts || {};

    Object.extend(window.mia.charts, {
        setupMinMaxAxis: function(config, l, dualaxis) {
            if (null == dualaxis) dualaxis = false;
            var limits = [];
            var values = this.getAllValues(config);
            var svalues = [];

            l.split("|").each(function(e) {
                a = e.split(",");
                limits.push({ min: a[0], max: a[1] });
            });

            var minvalue = Math.min.apply(Math, values);
            var maxvalue = Math.max.apply(Math, values);
            var sminvalue = 0;
            var smaxvalue = 0;

            if (dualaxis == true) {
                svalues = this.getDualValues(config);
                sminvalue = Math.min.apply(Math, svalues);
                smaxvalue = Math.max.apply(Math, svalues);
            }

            limits.each(function(l) {
                if (dualaxis == true) {
                    if (l.min <= minvalue && maxvalue <= l.max) {
                        config['chart'].PYAxisMaxValue = l.max;
                        config['chart'].PYAxisMinValue = l.min;
                    }
                    if (l.min <= sminvalue && smaxvalue <= l.max) {
                        config['chart'].SYAxisMaxValue = l.max;
                        config['chart'].SYAxisMinValue = l.min;
                    }
                }
                else {
                    if (l.min <= minvalue && maxvalue <= l.max) {
                        config['chart'].yAxisMaxValue = l.max;
                        config['chart'].yAxisMinValue = l.min;
                    }
                }
            });
        },

        getMinMaxAxis: function(config, l) {
            var limits = [];
            var values = [];
            config['dataset'].each(function(d) {
                d['data'].each(function(r) { values.push(parseInt(r.value)) });
            });

            l.split(",").each(function(e) {
                a = e.split(":");
                limits.push({ min: a[0], max: a[1] });
            });

            var minvalue = Math.min.apply(Math, values);
            var maxvalue = Math.max.apply(Math, values);
            var result = { min: 0, max: 0 };
            limits.each(function(l) {
                if (l.min <= minvalue && maxvalue <= l.max) {
                    result.max = l.max;
                    result.min = l.min;
                }
            });

            return result;
        },

        convertToProcents: function(config) {
            var values = this.getAllValues(config);
            var minvalue = Math.min.apply(Math, values);
            var maxvalue = Math.max.apply(Math, values);
            config['dataset'][0]['data'].each(function(r) {
                
            });
        },

        getAllValues: function(config) {
            var values = [];
            config['dataset'][0]['data'].each(function(r) { if (r.value != '') {
                if (r.value.indexOf('-.') == 0 || r.value.indexOf('.') == 0) r.value = r.value.replace('.', '0.');
                values.push(parseInt(r.value));
            }});
            return values;
        },

        getDualValues: function(config) {
            var values = [];
            config['dataset'].each(function(d, i) {
                if (i >= 1) {
                    d['data'].each(function(r) { if (r.value != '') {
                        if (r.value.indexOf('-.') == 0 || r.value.indexOf('.') == 0) r.value = r.value.replace('.', '0.');
                        values.push(parseInt(r.value));
                    }});
                }
            });
            return values;
        }
    });
})();