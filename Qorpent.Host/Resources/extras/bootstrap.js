/*!
 * Bootstrap v3.1.1 (http://getbootstrap.com)
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 */

if (typeof jQuery === 'undefined') { throw new Error('Bootstrap\'s JavaScript requires jQuery') }

/* ========================================================================
 * Bootstrap: transition.js v3.1.1
 * http://getbootstrap.com/javascript/#transitions
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // CSS TRANSITION SUPPORT (Shoutout: http://www.modernizr.com/)
  // ============================================================

  function transitionEnd() {
    var el = document.createElement('bootstrap')

    var transEndEventNames = {
      'WebkitTransition' : 'webkitTransitionEnd',
      'MozTransition'    : 'transitionend',
      'OTransition'      : 'oTransitionEnd otransitionend',
      'transition'       : 'transitionend'
    }

    for (var name in transEndEventNames) {
      if (el.style[name] !== undefined) {
        return { end: transEndEventNames[name] }
      }
    }

    return false // explicit for ie8 (  ._.)
  }

  // http://blog.alexmaccaw.com/css-transitions
  $.fn.emulateTransitionEnd = function (duration) {
    var called = false, $el = this
    $(this).one($.support.transition.end, function () { called = true })
    var callback = function () { if (!called) $($el).trigger($.support.transition.end) }
    setTimeout(callback, duration)
    return this
  }

  $(function () {
    $.support.transition = transitionEnd()
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: alert.js v3.1.1
 * http://getbootstrap.com/javascript/#alerts
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // ALERT CLASS DEFINITION
  // ======================

  var dismiss = '[data-dismiss="alert"]'
  var Alert   = function (el) {
    $(el).on('click', dismiss, this.close)
  }

  Alert.prototype.close = function (e) {
    var $this    = $(this)
    var selector = $this.attr('data-target')

    if (!selector) {
      selector = $this.attr('href')
      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
    }

    var $parent = $(selector)

    if (e) e.preventDefault()

    if (!$parent.length) {
      $parent = $this.hasClass('alert') ? $this : $this.parent()
    }

    $parent.trigger(e = $.Event('close.bs.alert'))

    if (e.isDefaultPrevented()) return

    $parent.removeClass('in')

    function removeElement() {
      $parent.trigger('closed.bs.alert').remove()
    }

    $.support.transition && $parent.hasClass('fade') ?
      $parent
        .one($.support.transition.end, removeElement)
        .emulateTransitionEnd(150) :
      removeElement()
  }


  // ALERT PLUGIN DEFINITION
  // =======================

  var old = $.fn.alert

  $.fn.alert = function (option) {
    return this.each(function () {
      var $this = $(this)
      var data  = $this.data('bs.alert')

      if (!data) $this.data('bs.alert', (data = new Alert(this)))
      if (typeof option == 'string') data[option].call($this)
    })
  }

  $.fn.alert.Constructor = Alert


  // ALERT NO CONFLICT
  // =================

  $.fn.alert.noConflict = function () {
    $.fn.alert = old
    return this
  }


  // ALERT DATA-API
  // ==============

  $(document).on('click.bs.alert.data-api', dismiss, Alert.prototype.close)

}(jQuery);

/* ========================================================================
 * Bootstrap: button.js v3.1.1
 * http://getbootstrap.com/javascript/#buttons
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // BUTTON PUBLIC CLASS DEFINITION
  // ==============================

  var Button = function (element, options) {
    this.$element  = $(element)
    this.options   = $.extend({}, Button.DEFAULTS, options)
    this.isLoading = false
  }

  Button.DEFAULTS = {
    loadingText: 'loading...'
  }

  Button.prototype.setState = function (state) {
    var d    = 'disabled'
    var $el  = this.$element
    var val  = $el.is('input') ? 'val' : 'html'
    var data = $el.data()

    state = state + 'Text'

    if (!data.resetText) $el.data('resetText', $el[val]())

    $el[val](data[state] || this.options[state])

    // push to event loop to allow forms to submit
    setTimeout($.proxy(function () {
      if (state == 'loadingText') {
        this.isLoading = true
        $el.addClass(d).attr(d, d)
      } else if (this.isLoading) {
        this.isLoading = false
        $el.removeClass(d).removeAttr(d)
      }
    }, this), 0)
  }

  Button.prototype.toggle = function () {
    var changed = true
    var $parent = this.$element.closest('[data-toggle="buttons"]')

    if ($parent.length) {
      var $input = this.$element.find('input')
      if ($input.prop('type') == 'radio') {
        if ($input.prop('checked') && this.$element.hasClass('active')) changed = false
        else $parent.find('.active').removeClass('active')
      }
      if (changed) $input.prop('checked', !this.$element.hasClass('active')).trigger('change')
    }

    if (changed) this.$element.toggleClass('active')
  }


  // BUTTON PLUGIN DEFINITION
  // ========================

  var old = $.fn.button

  $.fn.button = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.button')
      var options = typeof option == 'object' && option

      if (!data) $this.data('bs.button', (data = new Button(this, options)))

      if (option == 'toggle') data.toggle()
      else if (option) data.setState(option)
    })
  }

  $.fn.button.Constructor = Button


  // BUTTON NO CONFLICT
  // ==================

  $.fn.button.noConflict = function () {
    $.fn.button = old
    return this
  }


  // BUTTON DATA-API
  // ===============

  $(document).on('click.bs.button.data-api', '[data-toggle^=button]', function (e) {
    var $btn = $(e.target)
    if (!$btn.hasClass('btn')) $btn = $btn.closest('.btn')
    $btn.button('toggle')
    e.preventDefault()
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: carousel.js v3.1.1
 * http://getbootstrap.com/javascript/#carousel
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // CAROUSEL CLASS DEFINITION
  // =========================

  var Carousel = function (element, options) {
    this.$element    = $(element)
    this.$indicators = this.$element.find('.carousel-indicators')
    this.options     = options
    this.paused      =
    this.sliding     =
    this.interval    =
    this.$active     =
    this.$items      = null

    this.options.pause == 'hover' && this.$element
      .on('mouseenter', $.proxy(this.pause, this))
      .on('mouseleave', $.proxy(this.cycle, this))
  }

  Carousel.DEFAULTS = {
    interval: 5000,
    pause: 'hover',
    wrap: true
  }

  Carousel.prototype.cycle =  function (e) {
    e || (this.paused = false)

    this.interval && clearInterval(this.interval)

    this.options.interval
      && !this.paused
      && (this.interval = setInterval($.proxy(this.next, this), this.options.interval))

    return this
  }

  Carousel.prototype.getActiveIndex = function () {
    this.$active = this.$element.find('.item.active')
    this.$items  = this.$active.parent().children()

    return this.$items.index(this.$active)
  }

  Carousel.prototype.to = function (pos) {
    var that        = this
    var activeIndex = this.getActiveIndex()

    if (pos > (this.$items.length - 1) || pos < 0) return

    if (this.sliding)       return this.$element.one('slid.bs.carousel', function () { that.to(pos) })
    if (activeIndex == pos) return this.pause().cycle()

    return this.slide(pos > activeIndex ? 'next' : 'prev', $(this.$items[pos]))
  }

  Carousel.prototype.pause = function (e) {
    e || (this.paused = true)

    if (this.$element.find('.next, .prev').length && $.support.transition) {
      this.$element.trigger($.support.transition.end)
      this.cycle(true)
    }

    this.interval = clearInterval(this.interval)

    return this
  }

  Carousel.prototype.next = function () {
    if (this.sliding) return
    return this.slide('next')
  }

  Carousel.prototype.prev = function () {
    if (this.sliding) return
    return this.slide('prev')
  }

  Carousel.prototype.slide = function (type, next) {
    var $active   = this.$element.find('.item.active')
    var $next     = next || $active[type]()
    var isCycling = this.interval
    var direction = type == 'next' ? 'left' : 'right'
    var fallback  = type == 'next' ? 'first' : 'last'
    var that      = this

    if (!$next.length) {
      if (!this.options.wrap) return
      $next = this.$element.find('.item')[fallback]()
    }

    if ($next.hasClass('active')) return this.sliding = false

    var e = $.Event('slide.bs.carousel', { relatedTarget: $next[0], direction: direction })
    this.$element.trigger(e)
    if (e.isDefaultPrevented()) return

    this.sliding = true

    isCycling && this.pause()

    if (this.$indicators.length) {
      this.$indicators.find('.active').removeClass('active')
      this.$element.one('slid.bs.carousel', function () {
        var $nextIndicator = $(that.$indicators.children()[that.getActiveIndex()])
        $nextIndicator && $nextIndicator.addClass('active')
      })
    }

    if ($.support.transition && this.$element.hasClass('slide')) {
      $next.addClass(type)
      $next[0].offsetWidth // force reflow
      $active.addClass(direction)
      $next.addClass(direction)
      $active
        .one($.support.transition.end, function () {
          $next.removeClass([type, direction].join(' ')).addClass('active')
          $active.removeClass(['active', direction].join(' '))
          that.sliding = false
          setTimeout(function () { that.$element.trigger('slid.bs.carousel') }, 0)
        })
        .emulateTransitionEnd($active.css('transition-duration').slice(0, -1) * 1000)
    } else {
      $active.removeClass('active')
      $next.addClass('active')
      this.sliding = false
      this.$element.trigger('slid.bs.carousel')
    }

    isCycling && this.cycle()

    return this
  }


  // CAROUSEL PLUGIN DEFINITION
  // ==========================

  var old = $.fn.carousel

  $.fn.carousel = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.carousel')
      var options = $.extend({}, Carousel.DEFAULTS, $this.data(), typeof option == 'object' && option)
      var action  = typeof option == 'string' ? option : options.slide

      if (!data) $this.data('bs.carousel', (data = new Carousel(this, options)))
      if (typeof option == 'number') data.to(option)
      else if (action) data[action]()
      else if (options.interval) data.pause().cycle()
    })
  }

  $.fn.carousel.Constructor = Carousel


  // CAROUSEL NO CONFLICT
  // ====================

  $.fn.carousel.noConflict = function () {
    $.fn.carousel = old
    return this
  }


  // CAROUSEL DATA-API
  // =================

  $(document).on('click.bs.carousel.data-api', '[data-slide], [data-slide-to]', function (e) {
    var $this   = $(this), href
    var $target = $($this.attr('data-target') || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '')) //strip for ie7
    var options = $.extend({}, $target.data(), $this.data())
    var slideIndex = $this.attr('data-slide-to')
    if (slideIndex) options.interval = false

    $target.carousel(options)

    if (slideIndex = $this.attr('data-slide-to')) {
      $target.data('bs.carousel').to(slideIndex)
    }

    e.preventDefault()
  })

  $(window).on('load', function () {
    $('[data-ride="carousel"]').each(function () {
      var $carousel = $(this)
      $carousel.carousel($carousel.data())
    })
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: collapse.js v3.1.1
 * http://getbootstrap.com/javascript/#collapse
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // COLLAPSE PUBLIC CLASS DEFINITION
  // ================================

  var Collapse = function (element, options) {
    this.$element      = $(element)
    this.options       = $.extend({}, Collapse.DEFAULTS, options)
    this.transitioning = null

    if (this.options.parent) this.$parent = $(this.options.parent)
    if (this.options.toggle) this.toggle()
  }

  Collapse.DEFAULTS = {
    toggle: true
  }

  Collapse.prototype.dimension = function () {
    var hasWidth = this.$element.hasClass('width')
    return hasWidth ? 'width' : 'height'
  }

  Collapse.prototype.show = function () {
    if (this.transitioning || this.$element.hasClass('in')) return

    var startEvent = $.Event('show.bs.collapse')
    this.$element.trigger(startEvent)
    if (startEvent.isDefaultPrevented()) return

    var actives = this.$parent && this.$parent.find('> .panel > .in')

    if (actives && actives.length) {
      var hasData = actives.data('bs.collapse')
      if (hasData && hasData.transitioning) return
      actives.collapse('hide')
      hasData || actives.data('bs.collapse', null)
    }

    var dimension = this.dimension()

    this.$element
      .removeClass('collapse')
      .addClass('collapsing')
      [dimension](0)

    this.transitioning = 1

    var complete = function () {
      this.$element
        .removeClass('collapsing')
        .addClass('collapse in')
        [dimension]('auto')
      this.transitioning = 0
      this.$element.trigger('shown.bs.collapse')
    }

    if (!$.support.transition) return complete.call(this)

    var scrollSize = $.camelCase(['scroll', dimension].join('-'))

    this.$element
      .one($.support.transition.end, $.proxy(complete, this))
      .emulateTransitionEnd(350)
      [dimension](this.$element[0][scrollSize])
  }

  Collapse.prototype.hide = function () {
    if (this.transitioning || !this.$element.hasClass('in')) return

    var startEvent = $.Event('hide.bs.collapse')
    this.$element.trigger(startEvent)
    if (startEvent.isDefaultPrevented()) return

    var dimension = this.dimension()

    this.$element
      [dimension](this.$element[dimension]())
      [0].offsetHeight

    this.$element
      .addClass('collapsing')
      .removeClass('collapse')
      .removeClass('in')

    this.transitioning = 1

    var complete = function () {
      this.transitioning = 0
      this.$element
        .trigger('hidden.bs.collapse')
        .removeClass('collapsing')
        .addClass('collapse')
    }

    if (!$.support.transition) return complete.call(this)

    this.$element
      [dimension](0)
      .one($.support.transition.end, $.proxy(complete, this))
      .emulateTransitionEnd(350)
  }

  Collapse.prototype.toggle = function () {
    this[this.$element.hasClass('in') ? 'hide' : 'show']()
  }


  // COLLAPSE PLUGIN DEFINITION
  // ==========================

  var old = $.fn.collapse

  $.fn.collapse = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.collapse')
      var options = $.extend({}, Collapse.DEFAULTS, $this.data(), typeof option == 'object' && option)

      if (!data && options.toggle && option == 'show') option = !option
      if (!data) $this.data('bs.collapse', (data = new Collapse(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.collapse.Constructor = Collapse


  // COLLAPSE NO CONFLICT
  // ====================

  $.fn.collapse.noConflict = function () {
    $.fn.collapse = old
    return this
  }


  // COLLAPSE DATA-API
  // =================

  $(document).on('click.bs.collapse.data-api', '[data-toggle=collapse]', function (e) {
    var $this   = $(this), href
    var target  = $this.attr('data-target')
        || e.preventDefault()
        || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '') //strip for ie7
    var $target = $(target)
    var data    = $target.data('bs.collapse')
    var option  = data ? 'toggle' : $this.data()
    var parent  = $this.attr('data-parent')
    var $parent = parent && $(parent)

    if (!data || !data.transitioning) {
      if ($parent) $parent.find('[data-toggle=collapse][data-parent="' + parent + '"]').not($this).addClass('collapsed')
      $this[$target.hasClass('in') ? 'addClass' : 'removeClass']('collapsed')
    }

    $target.collapse(option)
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: dropdown.js v3.1.1
 * http://getbootstrap.com/javascript/#dropdowns
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // DROPDOWN CLASS DEFINITION
  // =========================

  var backdrop = '.dropdown-backdrop'
  var toggle   = '[data-toggle=dropdown]'
  var Dropdown = function (element) {
    $(element).on('click.bs.dropdown', this.toggle)
  }

  Dropdown.prototype.toggle = function (e) {
    var $this = $(this)

    if ($this.is('.disabled, :disabled')) return

    var $parent  = getParent($this)
    var isActive = $parent.hasClass('open')

    clearMenus()

    if (!isActive) {
      if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
        // if mobile we use a backdrop because click events don't delegate
        $('<div class="dropdown-backdrop"/>').insertAfter($(this)).on('click', clearMenus)
      }

      var relatedTarget = { relatedTarget: this }
      $parent.trigger(e = $.Event('show.bs.dropdown', relatedTarget))

      if (e.isDefaultPrevented()) return

      $parent
        .toggleClass('open')
        .trigger('shown.bs.dropdown', relatedTarget)

      $this.focus()
    }

    return false
  }

  Dropdown.prototype.keydown = function (e) {
    if (!/(38|40|27)/.test(e.keyCode)) return

    var $this = $(this)

    e.preventDefault()
    e.stopPropagation()

    if ($this.is('.disabled, :disabled')) return

    var $parent  = getParent($this)
    var isActive = $parent.hasClass('open')

    if (!isActive || (isActive && e.keyCode == 27)) {
      if (e.which == 27) $parent.find(toggle).focus()
      return $this.click()
    }

    var desc = ' li:not(.divider):visible a'
    var $items = $parent.find('[role=menu]' + desc + ', [role=listbox]' + desc)

    if (!$items.length) return

    var index = $items.index($items.filter(':focus'))

    if (e.keyCode == 38 && index > 0)                 index--                        // up
    if (e.keyCode == 40 && index < $items.length - 1) index++                        // down
    if (!~index)                                      index = 0

    $items.eq(index).focus()
  }

  function clearMenus(e) {
    $(backdrop).remove()
    $(toggle).each(function () {
      var $parent = getParent($(this))
      var relatedTarget = { relatedTarget: this }
      if (!$parent.hasClass('open')) return
      $parent.trigger(e = $.Event('hide.bs.dropdown', relatedTarget))
      if (e.isDefaultPrevented()) return
      $parent.removeClass('open').trigger('hidden.bs.dropdown', relatedTarget)
    })
  }

  function getParent($this) {
    var selector = $this.attr('data-target')

    if (!selector) {
      selector = $this.attr('href')
      selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') //strip for ie7
    }

    var $parent = selector && $(selector)

    return $parent && $parent.length ? $parent : $this.parent()
  }


  // DROPDOWN PLUGIN DEFINITION
  // ==========================

  var old = $.fn.dropdown

  $.fn.dropdown = function (option) {
    return this.each(function () {
      var $this = $(this)
      var data  = $this.data('bs.dropdown')

      if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
      if (typeof option == 'string') data[option].call($this)
    })
  }

  $.fn.dropdown.Constructor = Dropdown


  // DROPDOWN NO CONFLICT
  // ====================

  $.fn.dropdown.noConflict = function () {
    $.fn.dropdown = old
    return this
  }


  // APPLY TO STANDARD DROPDOWN ELEMENTS
  // ===================================

  $(document)
    .on('click.bs.dropdown.data-api', clearMenus)
    .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
    .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
    .on('keydown.bs.dropdown.data-api', toggle + ', [role=menu], [role=listbox]', Dropdown.prototype.keydown)

}(jQuery);

/* ========================================================================
 * Bootstrap: modal.js v3.1.1
 * http://getbootstrap.com/javascript/#modals
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // MODAL CLASS DEFINITION
  // ======================

  var Modal = function (element, options) {
    this.options   = options
    this.$element  = $(element)
    this.$backdrop =
    this.isShown   = null

    if (this.options.remote) {
      this.$element
        .find('.modal-content')
        .load(this.options.remote, $.proxy(function () {
          this.$element.trigger('loaded.bs.modal')
        }, this))
    }
  }

  Modal.DEFAULTS = {
    backdrop: true,
    keyboard: true,
    show: true
  }

  Modal.prototype.toggle = function (_relatedTarget) {
    return this[!this.isShown ? 'show' : 'hide'](_relatedTarget)
  }

  Modal.prototype.show = function (_relatedTarget) {
    var that = this
    var e    = $.Event('show.bs.modal', { relatedTarget: _relatedTarget })

    this.$element.trigger(e)

    if (this.isShown || e.isDefaultPrevented()) return

    this.isShown = true

    this.escape()

    this.$element.on('click.dismiss.bs.modal', '[data-dismiss="modal"]', $.proxy(this.hide, this))

    this.backdrop(function () {
      var transition = $.support.transition && that.$element.hasClass('fade')

      if (!that.$element.parent().length) {
        that.$element.appendTo(document.body) // don't move modals dom position
      }

      that.$element
        .show()
        .scrollTop(0)

      if (transition) {
        that.$element[0].offsetWidth // force reflow
      }

      that.$element
        .addClass('in')
        .attr('aria-hidden', false)

      that.enforceFocus()

      var e = $.Event('shown.bs.modal', { relatedTarget: _relatedTarget })

      transition ?
        that.$element.find('.modal-dialog') // wait for modal to slide in
          .one($.support.transition.end, function () {
            that.$element.focus().trigger(e)
          })
          .emulateTransitionEnd(300) :
        that.$element.focus().trigger(e)
    })
  }

  Modal.prototype.hide = function (e) {
    if (e) e.preventDefault()

    e = $.Event('hide.bs.modal')

    this.$element.trigger(e)

    if (!this.isShown || e.isDefaultPrevented()) return

    this.isShown = false

    this.escape()

    $(document).off('focusin.bs.modal')

    this.$element
      .removeClass('in')
      .attr('aria-hidden', true)
      .off('click.dismiss.bs.modal')

    $.support.transition && this.$element.hasClass('fade') ?
      this.$element
        .one($.support.transition.end, $.proxy(this.hideModal, this))
        .emulateTransitionEnd(300) :
      this.hideModal()
  }

  Modal.prototype.enforceFocus = function () {
    $(document)
      .off('focusin.bs.modal') // guard against infinite focus loop
      .on('focusin.bs.modal', $.proxy(function (e) {
        if (this.$element[0] !== e.target && !this.$element.has(e.target).length) {
          this.$element.focus()
        }
      }, this))
  }

  Modal.prototype.escape = function () {
    if (this.isShown && this.options.keyboard) {
      this.$element.on('keyup.dismiss.bs.modal', $.proxy(function (e) {
        e.which == 27 && this.hide()
      }, this))
    } else if (!this.isShown) {
      this.$element.off('keyup.dismiss.bs.modal')
    }
  }

  Modal.prototype.hideModal = function () {
    var that = this
    this.$element.hide()
    this.backdrop(function () {
      that.removeBackdrop()
      that.$element.trigger('hidden.bs.modal')
    })
  }

  Modal.prototype.removeBackdrop = function () {
    this.$backdrop && this.$backdrop.remove()
    this.$backdrop = null
  }

  Modal.prototype.backdrop = function (callback) {
    var animate = this.$element.hasClass('fade') ? 'fade' : ''

    if (this.isShown && this.options.backdrop) {
      var doAnimate = $.support.transition && animate

      this.$backdrop = $('<div class="modal-backdrop ' + animate + '" />')
        .appendTo(document.body)

      this.$element.on('click.dismiss.bs.modal', $.proxy(function (e) {
        if (e.target !== e.currentTarget) return
        this.options.backdrop == 'static'
          ? this.$element[0].focus.call(this.$element[0])
          : this.hide.call(this)
      }, this))

      if (doAnimate) this.$backdrop[0].offsetWidth // force reflow

      this.$backdrop.addClass('in')

      if (!callback) return

      doAnimate ?
        this.$backdrop
          .one($.support.transition.end, callback)
          .emulateTransitionEnd(150) :
        callback()

    } else if (!this.isShown && this.$backdrop) {
      this.$backdrop.removeClass('in')

      $.support.transition && this.$element.hasClass('fade') ?
        this.$backdrop
          .one($.support.transition.end, callback)
          .emulateTransitionEnd(150) :
        callback()

    } else if (callback) {
      callback()
    }
  }


  // MODAL PLUGIN DEFINITION
  // =======================

  var old = $.fn.modal

  $.fn.modal = function (option, _relatedTarget) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.modal')
      var options = $.extend({}, Modal.DEFAULTS, $this.data(), typeof option == 'object' && option)

      if (!data) $this.data('bs.modal', (data = new Modal(this, options)))
      if (typeof option == 'string') data[option](_relatedTarget)
      else if (options.show) data.show(_relatedTarget)
    })
  }

  $.fn.modal.Constructor = Modal


  // MODAL NO CONFLICT
  // =================

  $.fn.modal.noConflict = function () {
    $.fn.modal = old
    return this
  }


  // MODAL DATA-API
  // ==============

  $(document).on('click.bs.modal.data-api', '[data-toggle="modal"]', function (e) {
    var $this   = $(this)
    var href    = $this.attr('href')
    var $target = $($this.attr('data-target') || (href && href.replace(/.*(?=#[^\s]+$)/, ''))) //strip for ie7
    var option  = $target.data('bs.modal') ? 'toggle' : $.extend({ remote: !/#/.test(href) && href }, $target.data(), $this.data())

    if ($this.is('a')) e.preventDefault()

    $target
      .modal(option, this)
      .one('hide', function () {
        $this.is(':visible') && $this.focus()
      })
  })

  $(document)
    .on('show.bs.modal', '.modal', function () { $(document.body).addClass('modal-open') })
    .on('hidden.bs.modal', '.modal', function () { $(document.body).removeClass('modal-open') })

}(jQuery);

/* ========================================================================
 * Bootstrap: tooltip.js v3.1.1
 * http://getbootstrap.com/javascript/#tooltip
 * Inspired by the original jQuery.tipsy by Jason Frame
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // TOOLTIP PUBLIC CLASS DEFINITION
  // ===============================

  var Tooltip = function (element, options) {
    this.type       =
    this.options    =
    this.enabled    =
    this.timeout    =
    this.hoverState =
    this.$element   = null

    this.init('tooltip', element, options)
  }

  Tooltip.DEFAULTS = {
    animation: true,
    placement: 'top',
    selector: false,
    template: '<div class="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
    trigger: 'hover focus',
    title: '',
    delay: 0,
    html: false,
    container: false
  }

  Tooltip.prototype.init = function (type, element, options) {
    this.enabled  = true
    this.type     = type
    this.$element = $(element)
    this.options  = this.getOptions(options)

    var triggers = this.options.trigger.split(' ')

    for (var i = triggers.length; i--;) {
      var trigger = triggers[i]

      if (trigger == 'click') {
        this.$element.on('click.' + this.type, this.options.selector, $.proxy(this.toggle, this))
      } else if (trigger != 'manual') {
        var eventIn  = trigger == 'hover' ? 'mouseenter' : 'focusin'
        var eventOut = trigger == 'hover' ? 'mouseleave' : 'focusout'

        this.$element.on(eventIn  + '.' + this.type, this.options.selector, $.proxy(this.enter, this))
        this.$element.on(eventOut + '.' + this.type, this.options.selector, $.proxy(this.leave, this))
      }
    }

    this.options.selector ?
      (this._options = $.extend({}, this.options, { trigger: 'manual', selector: '' })) :
      this.fixTitle()
  }

  Tooltip.prototype.getDefaults = function () {
    return Tooltip.DEFAULTS
  }

  Tooltip.prototype.getOptions = function (options) {
    options = $.extend({}, this.getDefaults(), this.$element.data(), options)

    if (options.delay && typeof options.delay == 'number') {
      options.delay = {
        show: options.delay,
        hide: options.delay
      }
    }

    return options
  }

  Tooltip.prototype.getDelegateOptions = function () {
    var options  = {}
    var defaults = this.getDefaults()

    this._options && $.each(this._options, function (key, value) {
      if (defaults[key] != value) options[key] = value
    })

    return options
  }

  Tooltip.prototype.enter = function (obj) {
    var self = obj instanceof this.constructor ?
      obj : $(obj.currentTarget)[this.type](this.getDelegateOptions()).data('bs.' + this.type)

    clearTimeout(self.timeout)

    self.hoverState = 'in'

    if (!self.options.delay || !self.options.delay.show) return self.show()

    self.timeout = setTimeout(function () {
      if (self.hoverState == 'in') self.show()
    }, self.options.delay.show)
  }

  Tooltip.prototype.leave = function (obj) {
    var self = obj instanceof this.constructor ?
      obj : $(obj.currentTarget)[this.type](this.getDelegateOptions()).data('bs.' + this.type)

    clearTimeout(self.timeout)

    self.hoverState = 'out'

    if (!self.options.delay || !self.options.delay.hide) return self.hide()

    self.timeout = setTimeout(function () {
      if (self.hoverState == 'out') self.hide()
    }, self.options.delay.hide)
  }

  Tooltip.prototype.show = function () {
    var e = $.Event('show.bs.' + this.type)

    if (this.hasContent() && this.enabled) {
      this.$element.trigger(e)

      if (e.isDefaultPrevented()) return
      var that = this;

      var $tip = this.tip()

      this.setContent()

      if (this.options.animation) $tip.addClass('fade')

      var placement = typeof this.options.placement == 'function' ?
        this.options.placement.call(this, $tip[0], this.$element[0]) :
        this.options.placement

      var autoToken = /\s?auto?\s?/i
      var autoPlace = autoToken.test(placement)
      if (autoPlace) placement = placement.replace(autoToken, '') || 'top'

      $tip
        .detach()
        .css({ top: 0, left: 0, display: 'block' })
        .addClass(placement)

      this.options.container ? $tip.appendTo(this.options.container) : $tip.insertAfter(this.$element)

      var pos          = this.getPosition()
      var actualWidth  = $tip[0].offsetWidth
      var actualHeight = $tip[0].offsetHeight

      if (autoPlace) {
        var $parent = this.$element.parent()

        var orgPlacement = placement
        var docScroll    = document.documentElement.scrollTop || document.body.scrollTop
        var parentWidth  = this.options.container == 'body' ? window.innerWidth  : $parent.outerWidth()
        var parentHeight = this.options.container == 'body' ? window.innerHeight : $parent.outerHeight()
        var parentLeft   = this.options.container == 'body' ? 0 : $parent.offset().left

        placement = placement == 'bottom' && pos.top   + pos.height  + actualHeight - docScroll > parentHeight  ? 'top'    :
                    placement == 'top'    && pos.top   - docScroll   - actualHeight < 0                         ? 'bottom' :
                    placement == 'right'  && pos.right + actualWidth > parentWidth                              ? 'left'   :
                    placement == 'left'   && pos.left  - actualWidth < parentLeft                               ? 'right'  :
                    placement

        $tip
          .removeClass(orgPlacement)
          .addClass(placement)
      }

      var calculatedOffset = this.getCalculatedOffset(placement, pos, actualWidth, actualHeight)

      this.applyPlacement(calculatedOffset, placement)
      this.hoverState = null

      var complete = function() {
        that.$element.trigger('shown.bs.' + that.type)
      }

      $.support.transition && this.$tip.hasClass('fade') ?
        $tip
          .one($.support.transition.end, complete)
          .emulateTransitionEnd(150) :
        complete()
    }
  }

  Tooltip.prototype.applyPlacement = function (offset, placement) {
    var replace
    var $tip   = this.tip()
    var width  = $tip[0].offsetWidth
    var height = $tip[0].offsetHeight

    // manually read margins because getBoundingClientRect includes difference
    var marginTop = parseInt($tip.css('margin-top'), 10)
    var marginLeft = parseInt($tip.css('margin-left'), 10)

    // we must check for NaN for ie 8/9
    if (isNaN(marginTop))  marginTop  = 0
    if (isNaN(marginLeft)) marginLeft = 0

    offset.top  = offset.top  + marginTop
    offset.left = offset.left + marginLeft

    // $.fn.offset doesn't round pixel values
    // so we use setOffset directly with our own function B-0
    $.offset.setOffset($tip[0], $.extend({
      using: function (props) {
        $tip.css({
          top: Math.round(props.top),
          left: Math.round(props.left)
        })
      }
    }, offset), 0)

    $tip.addClass('in')

    // check to see if placing tip in new offset caused the tip to resize itself
    var actualWidth  = $tip[0].offsetWidth
    var actualHeight = $tip[0].offsetHeight

    if (placement == 'top' && actualHeight != height) {
      replace = true
      offset.top = offset.top + height - actualHeight
    }

    if (/bottom|top/.test(placement)) {
      var delta = 0

      if (offset.left < 0) {
        delta       = offset.left * -2
        offset.left = 0

        $tip.offset(offset)

        actualWidth  = $tip[0].offsetWidth
        actualHeight = $tip[0].offsetHeight
      }

      this.replaceArrow(delta - width + actualWidth, actualWidth, 'left')
    } else {
      this.replaceArrow(actualHeight - height, actualHeight, 'top')
    }

    if (replace) $tip.offset(offset)
  }

  Tooltip.prototype.replaceArrow = function (delta, dimension, position) {
    this.arrow().css(position, delta ? (50 * (1 - delta / dimension) + '%') : '')
  }

  Tooltip.prototype.setContent = function () {
    var $tip  = this.tip()
    var title = this.getTitle()

    $tip.find('.tooltip-inner')[this.options.html ? 'html' : 'text'](title)
    $tip.removeClass('fade in top bottom left right')
  }

  Tooltip.prototype.hide = function () {
    var that = this
    var $tip = this.tip()
    var e    = $.Event('hide.bs.' + this.type)

    function complete() {
      if (that.hoverState != 'in') $tip.detach()
      that.$element.trigger('hidden.bs.' + that.type)
    }

    this.$element.trigger(e)

    if (e.isDefaultPrevented()) return

    $tip.removeClass('in')

    $.support.transition && this.$tip.hasClass('fade') ?
      $tip
        .one($.support.transition.end, complete)
        .emulateTransitionEnd(150) :
      complete()

    this.hoverState = null

    return this
  }

  Tooltip.prototype.fixTitle = function () {
    var $e = this.$element
    if ($e.attr('title') || typeof($e.attr('data-original-title')) != 'string') {
      $e.attr('data-original-title', $e.attr('title') || '').attr('title', '')
    }
  }

  Tooltip.prototype.hasContent = function () {
    return this.getTitle()
  }

  Tooltip.prototype.getPosition = function () {
    var el = this.$element[0]
    return $.extend({}, (typeof el.getBoundingClientRect == 'function') ? el.getBoundingClientRect() : {
      width: el.offsetWidth,
      height: el.offsetHeight
    }, this.$element.offset())
  }

  Tooltip.prototype.getCalculatedOffset = function (placement, pos, actualWidth, actualHeight) {
    return placement == 'bottom' ? { top: pos.top + pos.height,   left: pos.left + pos.width / 2 - actualWidth / 2  } :
           placement == 'top'    ? { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2  } :
           placement == 'left'   ? { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth } :
        /* placement == 'right' */ { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width   }
  }

  Tooltip.prototype.getTitle = function () {
    var title
    var $e = this.$element
    var o  = this.options

    title = $e.attr('data-original-title')
      || (typeof o.title == 'function' ? o.title.call($e[0]) :  o.title)

    return title
  }

  Tooltip.prototype.tip = function () {
    return this.$tip = this.$tip || $(this.options.template)
  }

  Tooltip.prototype.arrow = function () {
    return this.$arrow = this.$arrow || this.tip().find('.tooltip-arrow')
  }

  Tooltip.prototype.validate = function () {
    if (!this.$element[0].parentNode) {
      this.hide()
      this.$element = null
      this.options  = null
    }
  }

  Tooltip.prototype.enable = function () {
    this.enabled = true
  }

  Tooltip.prototype.disable = function () {
    this.enabled = false
  }

  Tooltip.prototype.toggleEnabled = function () {
    this.enabled = !this.enabled
  }

  Tooltip.prototype.toggle = function (e) {
    var self = e ? $(e.currentTarget)[this.type](this.getDelegateOptions()).data('bs.' + this.type) : this
    self.tip().hasClass('in') ? self.leave(self) : self.enter(self)
  }

  Tooltip.prototype.destroy = function () {
    clearTimeout(this.timeout)
    this.hide().$element.off('.' + this.type).removeData('bs.' + this.type)
  }


  // TOOLTIP PLUGIN DEFINITION
  // =========================

  var old = $.fn.tooltip

  $.fn.tooltip = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.tooltip')
      var options = typeof option == 'object' && option

      if (!data && option == 'destroy') return
      if (!data) $this.data('bs.tooltip', (data = new Tooltip(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.tooltip.Constructor = Tooltip


  // TOOLTIP NO CONFLICT
  // ===================

  $.fn.tooltip.noConflict = function () {
    $.fn.tooltip = old
    return this
  }

}(jQuery);

/* ========================================================================
 * Bootstrap: popover.js v3.1.1
 * http://getbootstrap.com/javascript/#popovers
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // POPOVER PUBLIC CLASS DEFINITION
  // ===============================

  var Popover = function (element, options) {
    this.init('popover', element, options)
  }

  if (!$.fn.tooltip) throw new Error('Popover requires tooltip.js')

  Popover.DEFAULTS = $.extend({}, $.fn.tooltip.Constructor.DEFAULTS, {
    placement: 'right',
    trigger: 'click',
    content: '',
    template: '<div class="popover"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
  })


  // NOTE: POPOVER EXTENDS tooltip.js
  // ================================

  Popover.prototype = $.extend({}, $.fn.tooltip.Constructor.prototype)

  Popover.prototype.constructor = Popover

  Popover.prototype.getDefaults = function () {
    return Popover.DEFAULTS
  }

  Popover.prototype.setContent = function () {
    var $tip    = this.tip()
    var title   = this.getTitle()
    var content = this.getContent()

    $tip.find('.popover-title')[this.options.html ? 'html' : 'text'](title)
    $tip.find('.popover-content')[ // we use append for html objects to maintain js events
      this.options.html ? (typeof content == 'string' ? 'html' : 'append') : 'text'
    ](content)

    $tip.removeClass('fade top bottom left right in')

    // IE8 doesn't accept hiding via the `:empty` pseudo selector, we have to do
    // this manually by checking the contents.
    if (!$tip.find('.popover-title').html()) $tip.find('.popover-title').hide()
  }

  Popover.prototype.hasContent = function () {
    return this.getTitle() || this.getContent()
  }

  Popover.prototype.getContent = function () {
    var $e = this.$element
    var o  = this.options

    return $e.attr('data-content')
      || (typeof o.content == 'function' ?
            o.content.call($e[0]) :
            o.content)
  }

  Popover.prototype.arrow = function () {
    return this.$arrow = this.$arrow || this.tip().find('.arrow')
  }

  Popover.prototype.tip = function () {
    if (!this.$tip) this.$tip = $(this.options.template)
    return this.$tip
  }


  // POPOVER PLUGIN DEFINITION
  // =========================

  var old = $.fn.popover

  $.fn.popover = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.popover')
      var options = typeof option == 'object' && option

      if (!data && option == 'destroy') return
      if (!data) $this.data('bs.popover', (data = new Popover(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.popover.Constructor = Popover


  // POPOVER NO CONFLICT
  // ===================

  $.fn.popover.noConflict = function () {
    $.fn.popover = old
    return this
  }

}(jQuery);

/* ========================================================================
 * Bootstrap: scrollspy.js v3.1.1
 * http://getbootstrap.com/javascript/#scrollspy
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // SCROLLSPY CLASS DEFINITION
  // ==========================

  function ScrollSpy(element, options) {
    var href
    var process  = $.proxy(this.process, this)

    this.$element       = $(element).is('body') ? $(window) : $(element)
    this.$body          = $('body')
    this.$scrollElement = this.$element.on('scroll.bs.scroll-spy.data-api', process)
    this.options        = $.extend({}, ScrollSpy.DEFAULTS, options)
    this.selector       = (this.options.target
      || ((href = $(element).attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '')) //strip for ie7
      || '') + ' .nav li > a'
    this.offsets        = $([])
    this.targets        = $([])
    this.activeTarget   = null

    this.refresh()
    this.process()
  }

  ScrollSpy.DEFAULTS = {
    offset: 10
  }

  ScrollSpy.prototype.refresh = function () {
    var offsetMethod = this.$element[0] == window ? 'offset' : 'position'

    this.offsets = $([])
    this.targets = $([])

    var self     = this
    var $targets = this.$body
      .find(this.selector)
      .map(function () {
        var $el   = $(this)
        var href  = $el.data('target') || $el.attr('href')
        var $href = /^#./.test(href) && $(href)

        return ($href
          && $href.length
          && $href.is(':visible')
          && [[ $href[offsetMethod]().top + (!$.isWindow(self.$scrollElement.get(0)) && self.$scrollElement.scrollTop()), href ]]) || null
      })
      .sort(function (a, b) { return a[0] - b[0] })
      .each(function () {
        self.offsets.push(this[0])
        self.targets.push(this[1])
      })
  }

  ScrollSpy.prototype.process = function () {
    var scrollTop    = this.$scrollElement.scrollTop() + this.options.offset
    var scrollHeight = this.$scrollElement[0].scrollHeight || this.$body[0].scrollHeight
    var maxScroll    = scrollHeight - this.$scrollElement.height()
    var offsets      = this.offsets
    var targets      = this.targets
    var activeTarget = this.activeTarget
    var i

    if (scrollTop >= maxScroll) {
      return activeTarget != (i = targets.last()[0]) && this.activate(i)
    }

    if (activeTarget && scrollTop <= offsets[0]) {
      return activeTarget != (i = targets[0]) && this.activate(i)
    }

    for (i = offsets.length; i--;) {
      activeTarget != targets[i]
        && scrollTop >= offsets[i]
        && (!offsets[i + 1] || scrollTop <= offsets[i + 1])
        && this.activate( targets[i] )
    }
  }

  ScrollSpy.prototype.activate = function (target) {
    this.activeTarget = target

    $(this.selector)
      .parentsUntil(this.options.target, '.active')
      .removeClass('active')

    var selector = this.selector +
        '[data-target="' + target + '"],' +
        this.selector + '[href="' + target + '"]'

    var active = $(selector)
      .parents('li')
      .addClass('active')

    if (active.parent('.dropdown-menu').length) {
      active = active
        .closest('li.dropdown')
        .addClass('active')
    }

    active.trigger('activate.bs.scrollspy')
  }


  // SCROLLSPY PLUGIN DEFINITION
  // ===========================

  var old = $.fn.scrollspy

  $.fn.scrollspy = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.scrollspy')
      var options = typeof option == 'object' && option

      if (!data) $this.data('bs.scrollspy', (data = new ScrollSpy(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.scrollspy.Constructor = ScrollSpy


  // SCROLLSPY NO CONFLICT
  // =====================

  $.fn.scrollspy.noConflict = function () {
    $.fn.scrollspy = old
    return this
  }


  // SCROLLSPY DATA-API
  // ==================

  $(window).on('load', function () {
    $('[data-spy="scroll"]').each(function () {
      var $spy = $(this)
      $spy.scrollspy($spy.data())
    })
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: tab.js v3.1.1
 * http://getbootstrap.com/javascript/#tabs
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // TAB CLASS DEFINITION
  // ====================

  var Tab = function (element) {
    this.element = $(element)
  }

  Tab.prototype.show = function () {
    var $this    = this.element
    var $ul      = $this.closest('ul:not(.dropdown-menu)')
    var selector = $this.data('target')

    if (!selector) {
      selector = $this.attr('href')
      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') //strip for ie7
    }

    if ($this.parent('li').hasClass('active')) return

    var previous = $ul.find('.active:last a')[0]
    var e        = $.Event('show.bs.tab', {
      relatedTarget: previous
    })

    $this.trigger(e)

    if (e.isDefaultPrevented()) return

    var $target = $(selector)

    this.activate($this.parent('li'), $ul)
    this.activate($target, $target.parent(), function () {
      $this.trigger({
        type: 'shown.bs.tab',
        relatedTarget: previous
      })
    })
  }

  Tab.prototype.activate = function (element, container, callback) {
    var $active    = container.find('> .active')
    var transition = callback
      && $.support.transition
      && $active.hasClass('fade')

    function next() {
      $active
        .removeClass('active')
        .find('> .dropdown-menu > .active')
        .removeClass('active')

      element.addClass('active')

      if (transition) {
        element[0].offsetWidth // reflow for transition
        element.addClass('in')
      } else {
        element.removeClass('fade')
      }

      if (element.parent('.dropdown-menu')) {
        element.closest('li.dropdown').addClass('active')
      }

      callback && callback()
    }

    transition ?
      $active
        .one($.support.transition.end, next)
        .emulateTransitionEnd(150) :
      next()

    $active.removeClass('in')
  }


  // TAB PLUGIN DEFINITION
  // =====================

  var old = $.fn.tab

  $.fn.tab = function ( option ) {
    return this.each(function () {
      var $this = $(this)
      var data  = $this.data('bs.tab')

      if (!data) $this.data('bs.tab', (data = new Tab(this)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.tab.Constructor = Tab


  // TAB NO CONFLICT
  // ===============

  $.fn.tab.noConflict = function () {
    $.fn.tab = old
    return this
  }


  // TAB DATA-API
  // ============

  $(document).on('click.bs.tab.data-api', '[data-toggle="tab"], [data-toggle="pill"]', function (e) {
    e.preventDefault()
    $(this).tab('show')
  })

}(jQuery);

/* ========================================================================
 * Bootstrap: affix.js v3.1.1
 * http://getbootstrap.com/javascript/#affix
 * ========================================================================
 * Copyright 2011-2014 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
  'use strict';

  // AFFIX CLASS DEFINITION
  // ======================

  var Affix = function (element, options) {
    this.options = $.extend({}, Affix.DEFAULTS, options)
    this.$window = $(window)
      .on('scroll.bs.affix.data-api', $.proxy(this.checkPosition, this))
      .on('click.bs.affix.data-api',  $.proxy(this.checkPositionWithEventLoop, this))

    this.$element     = $(element)
    this.affixed      =
    this.unpin        =
    this.pinnedOffset = null

    this.checkPosition()
  }

  Affix.RESET = 'affix affix-top affix-bottom'

  Affix.DEFAULTS = {
    offset: 0
  }

  Affix.prototype.getPinnedOffset = function () {
    if (this.pinnedOffset) return this.pinnedOffset
    this.$element.removeClass(Affix.RESET).addClass('affix')
    var scrollTop = this.$window.scrollTop()
    var position  = this.$element.offset()
    return (this.pinnedOffset = position.top - scrollTop)
  }

  Affix.prototype.checkPositionWithEventLoop = function () {
    setTimeout($.proxy(this.checkPosition, this), 1)
  }

  Affix.prototype.checkPosition = function () {
    if (!this.$element.is(':visible')) return

    var scrollHeight = $(document).height()
    var scrollTop    = this.$window.scrollTop()
    var position     = this.$element.offset()
    var offset       = this.options.offset
    var offsetTop    = offset.top
    var offsetBottom = offset.bottom

    if (this.affixed == 'top') position.top += scrollTop

    if (typeof offset != 'object')         offsetBottom = offsetTop = offset
    if (typeof offsetTop == 'function')    offsetTop    = offset.top(this.$element)
    if (typeof offsetBottom == 'function') offsetBottom = offset.bottom(this.$element)

    var affix = this.unpin   != null && (scrollTop + this.unpin <= position.top) ? false :
                offsetBottom != null && (position.top + this.$element.height() >= scrollHeight - offsetBottom) ? 'bottom' :
                offsetTop    != null && (scrollTop <= offsetTop) ? 'top' : false

    if (this.affixed === affix) return
    if (this.unpin) this.$element.css('top', '')

    var affixType = 'affix' + (affix ? '-' + affix : '')
    var e         = $.Event(affixType + '.bs.affix')

    this.$element.trigger(e)

    if (e.isDefaultPrevented()) return

    this.affixed = affix
    this.unpin = affix == 'bottom' ? this.getPinnedOffset() : null

    this.$element
      .removeClass(Affix.RESET)
      .addClass(affixType)
      .trigger($.Event(affixType.replace('affix', 'affixed')))

    if (affix == 'bottom') {
      this.$element.offset({ top: scrollHeight - offsetBottom - this.$element.height() })
    }
  }


  // AFFIX PLUGIN DEFINITION
  // =======================

  var old = $.fn.affix

  $.fn.affix = function (option) {
    return this.each(function () {
      var $this   = $(this)
      var data    = $this.data('bs.affix')
      var options = typeof option == 'object' && option

      if (!data) $this.data('bs.affix', (data = new Affix(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.affix.Constructor = Affix


  // AFFIX NO CONFLICT
  // =================

  $.fn.affix.noConflict = function () {
    $.fn.affix = old
    return this
  }


  // AFFIX DATA-API
  // ==============

  $(window).on('load', function () {
    $('[data-spy="affix"]').each(function () {
      var $spy = $(this)
      var data = $spy.data()

      data.offset = data.offset || {}

      if (data.offsetBottom) data.offset.bottom = data.offsetBottom
      if (data.offsetTop)    data.offset.top    = data.offsetTop

      $spy.affix(data)
    })
  })

}(jQuery);


/* ========================================================================
 * bootstrap-switch - v3.0.0
 * http://www.bootstrap-switch.org
 * ========================================================================
 * Copyright 2012-2013 Mattia Larentis
 *
 * ========================================================================
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================================
 */

(function () {
    var __slice = [].slice;

    (function ($, window) {
        "use strict";
        var BootstrapSwitch;
        BootstrapSwitch = (function () {
            BootstrapSwitch.prototype.name = "bootstrap-switch";

            function BootstrapSwitch(element, options) {
                if (options == null) {
                    options = {};
                }
                this.$element = $(element);
                this.options = $.extend({}, $.fn.bootstrapSwitch.defaults, options, {
                    state: this.$element.is(":checked"),
                    size: this.$element.data("size"),
                    animate: this.$element.data("animate"),
                    disabled: this.$element.is(":disabled"),
                    readonly: this.$element.is("[readonly]"),
                    onColor: this.$element.data("on-color"),
                    offColor: this.$element.data("off-color"),
                    onText: this.$element.data("on-text"),
                    offText: this.$element.data("off-text"),
                    labelText: this.$element.data("label-text"),
                    baseClass: this.$element.data("base-class"),
                    wrapperClass: this.$element.data("wrapper-class")
                });
                this.$wrapper = $("<div>", {
                    "class": (function (_this) {
                        return function () {
                            var classes;
                            classes = ["" + _this.options.baseClass].concat(_this._getClasses(_this.options.wrapperClass));
                            classes.push(_this.options.state ? "" + _this.options.baseClass + "-on" : "" + _this.options.baseClass + "-off");
                            if (_this.options.size != null) {
                                classes.push("" + _this.options.baseClass + "-" + _this.options.size);
                            }
                            if (_this.options.animate) {
                                classes.push("" + _this.options.baseClass + "-animate");
                            }
                            if (_this.options.disabled) {
                                classes.push("" + _this.options.baseClass + "-disabled");
                            }
                            if (_this.options.readonly) {
                                classes.push("" + _this.options.baseClass + "-readonly");
                            }
                            if (_this.$element.attr("id")) {
                                classes.push("" + _this.options.baseClass + "-id-" + (_this.$element.attr("id")));
                            }
                            return classes.join(" ");
                        };
                    })(this)()
                });
                this.$container = $("<div>", {
                    "class": "" + this.options.baseClass + "-container"
                });
                this.$on = $("<span>", {
                    html: this.options.onText,
                    "class": "" + this.options.baseClass + "-handle-on " + this.options.baseClass + "-" + this.options.onColor
                });
                this.$off = $("<span>", {
                    html: this.options.offText,
                    "class": "" + this.options.baseClass + "-handle-off " + this.options.baseClass + "-" + this.options.offColor
                });
                this.$label = $("<label>", {
                    "for": this.$element.attr("id"),
                    html: this.options.labelText,
                    "class": "" + this.options.baseClass + "-label"
                });
                this.$element.on("init.bootstrapSwitch", (function (_this) {
                    return function () {
                        return _this.options.onInit.apply(element, arguments);
                    };
                })(this));
                this.$element.on("switchChange.bootstrapSwitch", (function (_this) {
                    return function () {
                        return _this.options.onSwitchChange.apply(element, arguments);
                    };
                })(this));
                this.$container = this.$element.wrap(this.$container).parent();
                this.$wrapper = this.$container.wrap(this.$wrapper).parent();
                this.$element.before(this.$on).before(this.$label).before(this.$off).trigger("init.bootstrapSwitch");
                this._elementHandlers();
                this._handleHandlers();
                this._labelHandlers();
                this._formHandler();
            }

            BootstrapSwitch.prototype._constructor = BootstrapSwitch;

            BootstrapSwitch.prototype.state = function (value, skip) {
                if (typeof value === "undefined") {
                    return this.options.state;
                }
                if (this.options.disabled || this.options.readonly) {
                    return this.$element;
                }
                value = !!value;
                this.$element.prop("checked", value).trigger("change.bootstrapSwitch", skip);
                return this.$element;
            };

            BootstrapSwitch.prototype.toggleState = function (skip) {
                if (this.options.disabled || this.options.readonly) {
                    return this.$element;
                }
                return this.$element.prop("checked", !this.options.state).trigger("change.bootstrapSwitch", skip);
            };

            BootstrapSwitch.prototype.size = function (value) {
                if (typeof value === "undefined") {
                    return this.options.size;
                }
                if (this.options.size != null) {
                    this.$wrapper.removeClass("" + this.options.baseClass + "-" + this.options.size);
                }
                if (value) {
                    this.$wrapper.addClass("" + this.options.baseClass + "-" + value);
                }
                this.options.size = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.animate = function (value) {
                if (typeof value === "undefined") {
                    return this.options.animate;
                }
                value = !!value;
                this.$wrapper[value ? "addClass" : "removeClass"]("" + this.options.baseClass + "-animate");
                this.options.animate = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.disabled = function (value) {
                if (typeof value === "undefined") {
                    return this.options.disabled;
                }
                value = !!value;
                this.$wrapper[value ? "addClass" : "removeClass"]("" + this.options.baseClass + "-disabled");
                this.$element.prop("disabled", value);
                this.options.disabled = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.toggleDisabled = function () {
                this.$element.prop("disabled", !this.options.disabled);
                this.$wrapper.toggleClass("" + this.options.baseClass + "-disabled");
                this.options.disabled = !this.options.disabled;
                return this.$element;
            };

            BootstrapSwitch.prototype.readonly = function (value) {
                if (typeof value === "undefined") {
                    return this.options.readonly;
                }
                value = !!value;
                this.$wrapper[value ? "addClass" : "removeClass"]("" + this.options.baseClass + "-readonly");
                this.$element.prop("readonly", value);
                this.options.readonly = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.toggleReadonly = function () {
                this.$element.prop("readonly", !this.options.readonly);
                this.$wrapper.toggleClass("" + this.options.baseClass + "-readonly");
                this.options.readonly = !this.options.readonly;
                return this.$element;
            };

            BootstrapSwitch.prototype.onColor = function (value) {
                var color;
                color = this.options.onColor;
                if (typeof value === "undefined") {
                    return color;
                }
                if (color != null) {
                    this.$on.removeClass("" + this.options.baseClass + "-" + color);
                }
                this.$on.addClass("" + this.options.baseClass + "-" + value);
                this.options.onColor = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.offColor = function (value) {
                var color;
                color = this.options.offColor;
                if (typeof value === "undefined") {
                    return color;
                }
                if (color != null) {
                    this.$off.removeClass("" + this.options.baseClass + "-" + color);
                }
                this.$off.addClass("" + this.options.baseClass + "-" + value);
                this.options.offColor = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.onText = function (value) {
                if (typeof value === "undefined") {
                    return this.options.onText;
                }
                this.$on.html(value);
                this.options.onText = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.offText = function (value) {
                if (typeof value === "undefined") {
                    return this.options.offText;
                }
                this.$off.html(value);
                this.options.offText = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.labelText = function (value) {
                if (typeof value === "undefined") {
                    return this.options.labelText;
                }
                this.$label.html(value);
                this.options.labelText = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.baseClass = function (value) {
                return this.options.baseClass;
            };

            BootstrapSwitch.prototype.wrapperClass = function (value) {
                if (typeof value === "undefined") {
                    return this.options.wrapperClass;
                }
                if (!value) {
                    value = $.fn.bootstrapSwitch.defaults.wrapperClass;
                }
                this.$wrapper.removeClass(this._getClasses(this.options.wrapperClass).join(" "));
                this.$wrapper.addClass(this._getClasses(value).join(" "));
                this.options.wrapperClass = value;
                return this.$element;
            };

            BootstrapSwitch.prototype.destroy = function () {
                var $form;
                $form = this.$element.closest("form");
                if ($form.length) {
                    $form.off("reset.bootstrapSwitch").removeData("bootstrap-switch");
                }
                this.$container.children().not(this.$element).remove();
                this.$element.unwrap().unwrap().off(".bootstrapSwitch").removeData("bootstrap-switch");
                return this.$element;
            };

            BootstrapSwitch.prototype._elementHandlers = function () {
                return this.$element.on({
                    "change.bootstrapSwitch": (function (_this) {
                        return function (e, skip) {
                            var checked;
                            e.preventDefault();
                            e.stopPropagation();
                            e.stopImmediatePropagation();
                            checked = _this.$element.is(":checked");
                            if (checked === _this.options.state) {
                                return;
                            }
                            _this.options.state = checked;
                            _this.$wrapper.removeClass(checked ? "" + _this.options.baseClass + "-off" : "" + _this.options.baseClass + "-on").addClass(checked ? "" + _this.options.baseClass + "-on" : "" + _this.options.baseClass + "-off");
                            if (!skip) {
                                if (_this.$element.is(":radio")) {
                                    $("[name='" + (_this.$element.attr('name')) + "']").not(_this.$element).prop("checked", false).trigger("change.bootstrapSwitch", true);
                                }
                                return _this.$element.trigger("switchChange.bootstrapSwitch", [checked]);
                            }
                        };
                    })(this),
                    "focus.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            e.preventDefault();
                            e.stopPropagation();
                            e.stopImmediatePropagation();
                            return _this.$wrapper.addClass("" + _this.options.baseClass + "-focused");
                        };
                    })(this),
                    "blur.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            e.preventDefault();
                            e.stopPropagation();
                            e.stopImmediatePropagation();
                            return _this.$wrapper.removeClass("" + _this.options.baseClass + "-focused");
                        };
                    })(this),
                    "keydown.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            if (!e.which || _this.options.disabled || _this.options.readonly) {
                                return;
                            }
                            switch (e.which) {
                                case 32:
                                    e.preventDefault();
                                    e.stopPropagation();
                                    e.stopImmediatePropagation();
                                    return _this.toggleState();
                                case 37:
                                    e.preventDefault();
                                    e.stopPropagation();
                                    e.stopImmediatePropagation();
                                    return _this.state(false);
                                case 39:
                                    e.preventDefault();
                                    e.stopPropagation();
                                    e.stopImmediatePropagation();
                                    return _this.state(true);
                            }
                        };
                    })(this)
                });
            };

            BootstrapSwitch.prototype._handleHandlers = function () {
                this.$on.on("click.bootstrapSwitch", (function (_this) {
                    return function (e) {
                        _this.state(false);
                        return _this.$element.trigger("focus.bootstrapSwitch");
                    };
                })(this));
                return this.$off.on("click.bootstrapSwitch", (function (_this) {
                    return function (e) {
                        _this.state(true);
                        return _this.$element.trigger("focus.bootstrapSwitch");
                    };
                })(this));
            };

            BootstrapSwitch.prototype._labelHandlers = function () {
                return this.$label.on({
                    "mousemove.bootstrapSwitch touchmove.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            var left, percent, right;
                            if (!_this.drag) {
                                return;
                            }
                            e.preventDefault();
                            percent = (((e.pageX || e.originalEvent.touches[0].pageX) - _this.$wrapper.offset().left) / _this.$wrapper.width()) * 100;
                            left = 25;
                            right = 75;
                            if (percent < left) {
                                percent = left;
                            } else if (percent > right) {
                                percent = right;
                            }
                            _this.$container.css("margin-left", "" + (percent - right) + "%");
                            return _this.$element.trigger("focus.bootstrapSwitch");
                        };
                    })(this),
                    "mousedown.bootstrapSwitch touchstart.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            if (_this.drag || _this.options.disabled || _this.options.readonly) {
                                return;
                            }
                            e.preventDefault();
                            _this.drag = true;
                            if (_this.options.animate) {
                                _this.$wrapper.removeClass("" + _this.options.baseClass + "-animate");
                            }
                            return _this.$element.trigger("focus.bootstrapSwitch");
                        };
                    })(this),
                    "mouseup.bootstrapSwitch touchend.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            if (!_this.drag) {
                                return;
                            }
                            e.preventDefault();
                            _this.drag = false;
                            _this.$element.prop("checked", parseInt(_this.$container.css("margin-left"), 10) > -(_this.$container.width() / 6)).trigger("change.bootstrapSwitch");
                            _this.$container.css("margin-left", "");
                            if (_this.options.animate) {
                                return _this.$wrapper.addClass("" + _this.options.baseClass + "-animate");
                            }
                        };
                    })(this),
                    "mouseleave.bootstrapSwitch": (function (_this) {
                        return function (e) {
                            return _this.$label.trigger("mouseup.bootstrapSwitch");
                        };
                    })(this)
                });
            };

            BootstrapSwitch.prototype._formHandler = function () {
                var $form;
                $form = this.$element.closest("form");
                if ($form.data("bootstrap-switch")) {
                    return;
                }
                return $form.on("reset.bootstrapSwitch", function () {
                    return window.setTimeout(function () {
                        return $form.find("input").filter(function () {
                            return $(this).data("bootstrap-switch");
                        }).each(function () {
                            return $(this).bootstrapSwitch("state", this.checked);
                        });
                    }, 1);
                }).data("bootstrap-switch", true);
            };

            BootstrapSwitch.prototype._getClasses = function (classes) {
                var c, cls, _i, _len;
                if (!$.isArray(classes)) {
                    return ["" + this.options.baseClass + "-" + classes];
                }
                cls = [];
                for (_i = 0, _len = classes.length; _i < _len; _i++) {
                    c = classes[_i];
                    cls.push("" + this.options.baseClass + "-" + c);
                }
                return cls;
            };

            return BootstrapSwitch;

        })();
        $.fn.bootstrapSwitch = function () {
            var args, option, ret;
            option = arguments[0], args = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
            ret = this;
            this.each(function () {
                var $this, data;
                $this = $(this);
                data = $this.data("bootstrap-switch");
                if (!data) {
                    $this.data("bootstrap-switch", data = new BootstrapSwitch(this, option));
                }
                if (typeof option === "string") {
                    return ret = data[option].apply(data, args);
                }
            });
            return ret;
        };
        $.fn.bootstrapSwitch.Constructor = BootstrapSwitch;
        return $.fn.bootstrapSwitch.defaults = {
            state: true,
            size: null,
            animate: true,
            disabled: false,
            readonly: false,
            onColor: "primary",
            offColor: "default",
            onText: "ON",
            offText: "OFF",
            labelText: "&nbsp;",
            baseClass: "bootstrap-switch",
            wrapperClass: "wrapper",
            onInit: function () { },
            onSwitchChange: function () { }
        };
    })(window.jQuery, window);

}).call(this);


/*
Version 3.0.0
=========================================================
bootstrap-datetimepicker.js
https://github.com/Eonasdan/bootstrap-datetimepicker
=========================================================
The MIT License (MIT)

Copyright (c) 2014 Jonathan Peterson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
!function (e) {
    if ("function" == typeof define && define.amd) define(["jquery", "moment"], e)
    else {
        if (!jQuery) throw "bootstrap-datetimepicker requires jQuery to be loaded first"
        if (!moment) throw "bootstrap-datetimepicker requires moment.js to be loaded first"
        e(jQuery, moment)
    }
}(function (e, t) {
    if (void 0 === t) throw alert("momentjs is requried"), Error("momentjs is required")
    var a = 0, o = t, n = function (t, n) {
        var s = { pickDate: !0, pickTime: !0, useMinutes: !0, useSeconds: !1, useCurrent: !0, minuteStepping: 1, minDate: new o({ y: 1900 }), maxDate: (new o).add(100, "y"), showToday: !0, collapse: !0, language: "en", defaultDate: "", disabledDates: !1, enabledDates: !1, icons: {}, useStrict: !1, direction: "auto", sideBySide: !1, daysOfWeekDisabled: !1 }, d = { time: "glyphicon glyphicon-time", date: "glyphicon glyphicon-calendar", up: "glyphicon glyphicon-chevron-up", down: "glyphicon glyphicon-chevron-down" }, r = this, c = function () {
            var i, c = !1
            if (r.options = e.extend({}, s, n), r.options.icons = e.extend({}, d, r.options.icons), r.element = e(t), l(), !r.options.pickTime && !r.options.pickDate) throw Error("Must choose at least one picker")
            if (r.id = a++, o.lang(r.options.language), r.date = o(), r.unset = !1, r.isInput = r.element.is("input"), r.component = !1, r.element.hasClass("input-group") && (r.component = r.element.find(0 == r.element.find(".datepickerbutton").size() ? "[class^='input-group-']" : ".datepickerbutton")), r.format = r.options.format, i = o()._lang._longDateFormat, r.format || (r.format = r.options.pickDate ? i.L : "", r.options.pickDate && r.options.pickTime && (r.format += " "), r.format += r.options.pickTime ? i.LT : "", r.options.useSeconds && (~i.LT.indexOf(" A") ? r.format = r.format.split(" A")[0] + ":ss A" : r.format += ":ss")), r.use24hours = r.format.toLowerCase().indexOf("a") < 1, r.component && (c = r.component.find("span")), r.options.pickTime && c && c.addClass(r.options.icons.time), r.options.pickDate && c && (c.removeClass(r.options.icons.time), c.addClass(r.options.icons.date)), r.widget = e(W()).appendTo("body"), r.options.useSeconds && !r.use24hours && r.widget.width(300), r.minViewMode = r.options.minViewMode || 0, "string" == typeof r.minViewMode) switch (r.minViewMode) {
                case "months": r.minViewMode = 1
                    break
                case "years": r.minViewMode = 2
                    break
                default: r.minViewMode = 0
            } if (r.viewMode = r.options.viewMode || 0, "string" == typeof r.viewMode) switch (r.viewMode) {
                case "months": r.viewMode = 1
                    break
                case "years": r.viewMode = 2
                    break
                default: r.viewMode = 0
            } if (r.options.disabledDates = j(r.options.disabledDates), r.options.enabledDates = j(r.options.enabledDates), r.startViewMode = r.viewMode, r.setMinDate(r.options.minDate), r.setMaxDate(r.options.maxDate), g(), w(), k(), b(), y(), h(), P(), V(), "" !== r.options.defaultDate && "" == p().val() && r.setValue(r.options.defaultDate), 1 !== r.options.minuteStepping) {
                var m = r.options.minuteStepping
                r.date.minutes(Math.round(r.date.minutes() / m) * m % 60).seconds(0)
            }
        }, p = function () { return r.isInput ? r.element : dateStr = r.element.find("input") }, l = function () {
            var e
            e = (r.element.is("input"), r.element.data()), void 0 !== e.dateFormat && (r.options.format = e.dateFormat), void 0 !== e.datePickdate && (r.options.pickDate = e.datePickdate), void 0 !== e.datePicktime && (r.options.pickTime = e.datePicktime), void 0 !== e.dateUseminutes && (r.options.useMinutes = e.dateUseminutes), void 0 !== e.dateUseseconds && (r.options.useSeconds = e.dateUseseconds), void 0 !== e.dateUsecurrent && (r.options.useCurrent = e.dateUsecurrent), void 0 !== e.dateMinutestepping && (r.options.minuteStepping = e.dateMinutestepping), void 0 !== e.dateMindate && (r.options.minDate = e.dateMindate), void 0 !== e.dateMaxdate && (r.options.maxDate = e.dateMaxdate), void 0 !== e.dateShowtoday && (r.options.showToday = e.dateShowtoday), void 0 !== e.dateCollapse && (r.options.collapse = e.dateCollapse), void 0 !== e.dateLanguage && (r.options.language = e.dateLanguage), void 0 !== e.dateDefaultdate && (r.options.defaultDate = e.dateDefaultdate), void 0 !== e.dateDisableddates && (r.options.disabledDates = e.dateDisableddates), void 0 !== e.dateEnableddates && (r.options.enabledDates = e.dateEnableddates), void 0 !== e.dateIcons && (r.options.icons = e.dateIcons), void 0 !== e.dateUsestrict && (r.options.useStrict = e.dateUsestrict), void 0 !== e.dateDirection && (r.options.direction = e.dateDirection), void 0 !== e.dateSidebyside && (r.options.sideBySide = e.dateSidebyside)
        }, m = function () {
            var t = "absolute", i = r.component ? r.component.offset() : r.element.offset(), a = e(window)
            r.width = r.component ? r.component.outerWidth() : r.element.outerWidth(), i.top = i.top + r.element.outerHeight()
            var o
            "up" === r.options.direction ? o = "top" : "bottom" === r.options.direction ? o = "bottom" : "auto" === r.options.direction && (o = i.top + r.widget.height() > a.height() + a.scrollTop() && r.widget.height() + r.element.outerHeight() < i.top ? "top" : "bottom"), "top" === o ? (i.top -= r.widget.height() + r.element.outerHeight() + 15, r.widget.addClass("top").removeClass("bottom")) : (i.top += 1, r.widget.addClass("bottom").removeClass("top")), void 0 !== r.options.width && r.widget.width(r.options.width), "left" === r.options.orientation && (r.widget.addClass("left-oriented"), i.left = i.left - r.widget.width() + 20), Y() && (t = "fixed", i.top -= a.scrollTop(), i.left -= a.scrollLeft()), a.width() < i.left + r.widget.outerWidth() ? (i.right = a.width() - i.left - r.width, i.left = "auto", r.widget.addClass("pull-right")) : (i.right = "auto", r.widget.removeClass("pull-right")), r.widget.css({ position: t, top: i.top, left: i.left, right: i.right })
        }, u = function (e, t) { o(r.date).isSame(o(e)) || (r.element.trigger({ type: "dp.change", date: o(r.date), oldDate: o(e) }), "change" !== t && r.element.change()) }, f = function (e) { r.element.trigger({ type: "dp.error", date: o(e) }) }, h = function (e) {
            o.lang(r.options.language)
            var t = e
            t || (t = p().val(), t && (r.date = o(t, r.format, r.options.useStrict)), r.date || (r.date = o())), r.viewDate = o(r.date).startOf("month"), v(), D()
        }, g = function () {
            o.lang(r.options.language)
            var t, i = e("<tr>"), a = o.weekdaysMin()
            if (0 == o()._lang._week.dow) for (t = 0; 7 > t; t++) i.append('<th class="dow">' + a[t] + "</th>")
            else for (t = 1; 8 > t; t++) i.append(7 == t ? '<th class="dow">' + a[0] + "</th>" : '<th class="dow">' + a[t] + "</th>")
            r.widget.find(".datepicker-days thead").append(i)
        }, w = function () {
            o.lang(r.options.language)
            for (var e = "", t = 0, i = o.monthsShort() ; 12 > t;) e += '<span class="month">' + i[t++] + "</span>"
            r.widget.find(".datepicker-months td").append(e)
        }, v = function () {
            o.lang(r.options.language)
            var t, i, a, n, s, d, c, p, l = r.viewDate.year(), m = r.viewDate.month(), u = r.options.minDate.year(), f = r.options.minDate.month(), h = r.options.maxDate.year(), g = r.options.maxDate.month(), w = [], v = o.months()
            for (r.widget.find(".datepicker-days").find(".disabled").removeClass("disabled"), r.widget.find(".datepicker-months").find(".disabled").removeClass("disabled"), r.widget.find(".datepicker-years").find(".disabled").removeClass("disabled"), r.widget.find(".datepicker-days th:eq(1)").text(v[m] + " " + l), t = o(r.viewDate).subtract("months", 1), d = t.daysInMonth(), t.date(d).startOf("week"), (l == u && f >= m || u > l) && r.widget.find(".datepicker-days th:eq(0)").addClass("disabled"), (l == h && m >= g || l > h) && r.widget.find(".datepicker-days th:eq(2)").addClass("disabled"), i = o(t).add(42, "d") ; t.isBefore(i) ;) {
                if (t.weekday() === o().startOf("week").weekday() && (a = e("<tr>"), w.push(a)), n = "", t.year() < l || t.year() == l && t.month() < m ? n += " old" : (t.year() > l || t.year() == l && t.month() > m) && (n += " new"), t.isSame(o({ y: r.date.year(), M: r.date.month(), d: r.date.date() })) && (n += " active"), (N(t) || !U(t)) && (n += " disabled"), r.options.showToday === !0 && t.isSame(o(), "day") && (n += " today"), r.options.daysOfWeekDisabled) for (s in r.options.daysOfWeekDisabled) if (t.day() == r.options.daysOfWeekDisabled[s]) {
                    n += " disabled"
                    break
                } a.append('<td class="day' + n + '">' + t.date() + "</td>"), t.add(1, "d")
            } for (r.widget.find(".datepicker-days tbody").empty().append(w), p = r.date.year(), v = r.widget.find(".datepicker-months").find("th:eq(1)").text(l).end().find("span").removeClass("active"), p === l && v.eq(r.date.month()).addClass("active"), u > p - 1 && r.widget.find(".datepicker-months th:eq(0)").addClass("disabled"), p + 1 > h && r.widget.find(".datepicker-months th:eq(2)").addClass("disabled"), s = 0; 12 > s; s++) l == u && f > s || u > l ? e(v[s]).addClass("disabled") : (l == h && s > g || l > h) && e(v[s]).addClass("disabled")
            for (w = "", l = 10 * parseInt(l / 10, 10), c = r.widget.find(".datepicker-years").find("th:eq(1)").text(l + "-" + (l + 9)).end().find("td"), r.widget.find(".datepicker-years").find("th").removeClass("disabled"), u > l && r.widget.find(".datepicker-years").find("th:eq(0)").addClass("disabled"), l + 9 > h && r.widget.find(".datepicker-years").find("th:eq(2)").addClass("disabled"), l -= 1, s = -1; 11 > s; s++) w += '<span class="year' + (-1 === s || 10 === s ? " old" : "") + (p === l ? " active" : "") + (u > l || l > h ? " disabled" : "") + '">' + l + "</span>", l += 1
            c.html(w)
        }, k = function () {
            o.lang(r.options.language)
            var e, t, i, a = r.widget.find(".timepicker .timepicker-hours table"), n = ""
            if (a.parent().hide(), r.use24hours) for (e = 0, t = 0; 6 > t; t += 1) {
                for (n += "<tr>", i = 0; 4 > i; i += 1) n += '<td class="hour">' + F("" + e) + "</td>", e++
                n += "</tr>"
            } else for (e = 1, t = 0; 3 > t; t += 1) {
                for (n += "<tr>", i = 0; 4 > i; i += 1) n += '<td class="hour">' + F("" + e) + "</td>", e++
                n += "</tr>"
            } a.html(n)
        }, b = function () {
            var e, t, i = r.widget.find(".timepicker .timepicker-minutes table"), a = "", o = 0, n = r.options.minuteStepping
            for (i.parent().hide(), 1 == n && (n = 5), e = 0; e < Math.ceil(60 / n / 4) ; e++) {
                for (a += "<tr>", t = 0; 4 > t; t += 1) 60 > o ? (a += '<td class="minute">' + F("" + o) + "</td>", o += n) : a += "<td></td>"
                a += "</tr>"
            } i.html(a)
        }, y = function () {
            var e, t, i = r.widget.find(".timepicker .timepicker-seconds table"), a = "", o = 0
            for (i.parent().hide(), e = 0; 3 > e; e++) {
                for (a += "<tr>", t = 0; 4 > t; t += 1) a += '<td class="second">' + F("" + o) + "</td>", o += 5
                a += "</tr>"
            } i.html(a)
        }, D = function () {
            if (r.date) {
                var e = r.widget.find(".timepicker span[data-time-component]"), t = r.date.hours(), i = "AM"
                r.use24hours || (t >= 12 && (i = "PM"), 0 === t ? t = 12 : 12 != t && (t %= 12), r.widget.find(".timepicker [data-action=togglePeriod]").text(i)), e.filter("[data-time-component=hours]").text(F(t)), e.filter("[data-time-component=minutes]").text(F(r.date.minutes())), e.filter("[data-time-component=seconds]").text(F(r.date.second()))
            }
        }, M = function (t) {
            t.stopPropagation(), t.preventDefault(), r.unset = !1
            var i, a, n, s, d = e(t.target).closest("span, td, th"), c = o(r.date)
            if (1 === d.length && !d.is(".disabled")) switch (d[0].nodeName.toLowerCase()) {
                case "th": switch (d[0].className) {
                    case "switch": P(1)
                        break
                    case "prev": case "next": n = B.modes[r.viewMode].navStep, "prev" === d[0].className && (n = -1 * n), r.viewDate.add(n, B.modes[r.viewMode].navFnc), v()
                } break
                case "span": d.is(".month") ? (i = d.parent().find("span").index(d), r.viewDate.month(i)) : (a = parseInt(d.text(), 10) || 0, r.viewDate.year(a)), r.viewMode === r.minViewMode && (r.date = o({ y: r.viewDate.year(), M: r.viewDate.month(), d: r.viewDate.date(), h: r.date.hours(), m: r.date.minutes(), s: r.date.seconds() }), u(c, t.type), O()), P(-1), v()
                    break
                case "td": d.is(".day") && (s = parseInt(d.text(), 10) || 1, i = r.viewDate.month(), a = r.viewDate.year(), d.is(".old") ? 0 === i ? (i = 11, a -= 1) : i -= 1 : d.is(".new") && (11 == i ? (i = 0, a += 1) : i += 1), r.date = o({ y: a, M: i, d: s, h: r.date.hours(), m: r.date.minutes(), s: r.date.seconds() }), r.viewDate = o({ y: a, M: i, d: Math.min(28, s) }), v(), O(), u(c, t.type))
            }
        }, x = {
            incrementHours: function () { L("add", "hours", 1) }, incrementMinutes: function () { L("add", "minutes", r.options.minuteStepping) }, incrementSeconds: function () { L("add", "seconds", 1) }, decrementHours: function () { L("subtract", "hours", 1) }, decrementMinutes: function () { L("subtract", "minutes", r.options.minuteStepping) }, decrementSeconds: function () { L("subtract", "seconds", 1) }, togglePeriod: function () {
                var e = r.date.hours()
                e >= 12 ? e -= 12 : e += 12, r.date.hours(e)
            }, showPicker: function () { r.widget.find(".timepicker > div:not(.timepicker-picker)").hide(), r.widget.find(".timepicker .timepicker-picker").show() }, showHours: function () { r.widget.find(".timepicker .timepicker-picker").hide(), r.widget.find(".timepicker .timepicker-hours").show() }, showMinutes: function () { r.widget.find(".timepicker .timepicker-picker").hide(), r.widget.find(".timepicker .timepicker-minutes").show() }, showSeconds: function () { r.widget.find(".timepicker .timepicker-picker").hide(), r.widget.find(".timepicker .timepicker-seconds").show() }, selectHour: function (t) {
                var i = r.widget.find(".timepicker [data-action=togglePeriod]").text(), a = parseInt(e(t.target).text(), 10)
                "PM" == i && (a += 12), r.date.hours(a), x.showPicker.call(r)
            }, selectMinute: function (t) { r.date.minutes(parseInt(e(t.target).text(), 10)), x.showPicker.call(r) }, selectSecond: function (t) { r.date.seconds(parseInt(e(t.target).text(), 10)), x.showPicker.call(r) }
        }, S = function (t) {
            var i = o(r.date), a = e(t.currentTarget).data("action"), n = x[a].apply(r, arguments)
            return T(t), r.date || (r.date = o({ y: 1970 })), O(), D(), u(i, t.type), n
        }, T = function (e) { e.stopPropagation(), e.preventDefault() }, C = function (t) {
            o.lang(r.options.language)
            var i = e(t.target), a = o(r.date), n = o(i.val(), r.format, r.options.useStrict)
            n.isValid() && !N(n) && U(n) ? (h(), r.setValue(n), u(a, t.type), O()) : (r.viewDate = a, u(a, t.type), f(n), r.unset = !0)
        }, P = function (e) {
            e && (r.viewMode = Math.max(r.minViewMode, Math.min(2, r.viewMode + e)))
            B.modes[r.viewMode].clsName
            r.widget.find(".datepicker > div").hide().filter(".datepicker-" + B.modes[r.viewMode].clsName).show()
        }, V = function () {
            var t, i, a, o, n
            r.widget.on("click", ".datepicker *", e.proxy(M, this)), r.widget.on("click", "[data-action]", e.proxy(S, this)), r.widget.on("mousedown", e.proxy(T, this)), r.options.pickDate && r.options.pickTime && r.widget.on("click.togglePicker", ".accordion-toggle", function (s) {
                if (s.stopPropagation(), t = e(this), i = t.closest("ul"), a = i.find(".in"), o = i.find(".collapse:not(.in)"), a && a.length) {
                    if (n = a.data("collapse"), n && n.date - transitioning) return
                    a.collapse("hide"), o.collapse("show"), t.find("span").toggleClass(r.options.icons.time + " " + r.options.icons.date), r.element.find(".input-group-addon span").toggleClass(r.options.icons.time + " " + r.options.icons.date)
                }
            }), r.isInput ? r.element.on({ focus: e.proxy(r.show, this), change: e.proxy(C, this), blur: e.proxy(r.hide, this) }) : (r.element.on({ change: e.proxy(C, this) }, "input"), r.component ? r.component.on("click", e.proxy(r.show, this)) : r.element.on("click", e.proxy(r.show, this)))
        }, q = function () { e(window).on("resize.datetimepicker" + r.id, e.proxy(m, this)), r.isInput || e(document).on("mousedown.datetimepicker" + r.id, e.proxy(r.hide, this)) }, I = function () { r.widget.off("click", ".datepicker *", r.click), r.widget.off("click", "[data-action]"), r.widget.off("mousedown", r.stopEvent), r.options.pickDate && r.options.pickTime && r.widget.off("click.togglePicker"), r.isInput ? r.element.off({ focus: r.show, change: r.change }) : (r.element.off({ change: r.change }, "input"), r.component ? r.component.off("click", r.show) : r.element.off("click", r.show)) }, H = function () { e(window).off("resize.datetimepicker" + r.id), r.isInput || e(document).off("mousedown.datetimepicker" + r.id) }, Y = function () {
            if (r.element) {
                var t, i = r.element.parents(), a = !1
                for (t = 0; t < i.length; t++) if ("fixed" == e(i[t]).css("position")) {
                    a = !0
                    break
                } return a
            } return !1
        }, O = function () {
            o.lang(r.options.language)
            var e = ""
            r.unset || (e = o(r.date).format(r.format)), p().val(e), r.element.data("date", e), r.options.pickTime || r.hide()
        }, L = function (e, t, i) {
            o.lang(r.options.language)
            var a
            return "add" == e ? (a = o(r.date), 23 == a.hours() && a.add(i, t), a.add(i, t)) : a = o(r.date).subtract(i, t), N(o(a.subtract(i, t))) || N(a) ? void f(a.format(r.format)) : ("add" == e ? r.date.add(i, t) : r.date.subtract(i, t), void (r.unset = !1))
        }, N = function (e) { return o.lang(r.options.language), e.isAfter(r.options.maxDate) || e.isBefore(r.options.minDate) ? !0 : r.options.disabledDates === !1 ? !1 : r.options.disabledDates[o(e).format("YYYY-MM-DD")] === !0 }, U = function (e) { return o.lang(r.options.language), r.options.enabledDates === !1 ? !0 : r.options.enabledDates[o(e).format("YYYY-MM-DD")] === !0 }, j = function (e) {
            var t = {}, a = 0
            for (i = 0; i < e.length; i++) dDate = o(e[i]), dDate.isValid() && (t[dDate.format("YYYY-MM-DD")] = !0, a++)
            return a > 0 ? t : !1
        }, F = function (e) { return e = "" + e, e.length >= 2 ? e : "0" + e }, W = function () {
            if (r.options.pickDate && r.options.pickTime) {
                var e = ""
                return e = '<div class="bootstrap-datetimepicker-widget' + (r.options.sideBySide ? " timepicker-sbs" : "") + ' dropdown-menu" style="z-index:9999 !important;">', e += r.options.sideBySide ? '<div class="row"><div class="col-sm-6 datepicker">' + B.template + '</div><div class="col-sm-6 timepicker">' + E.getTemplate() + "</div></div>" : '<ul class="list-unstyled"><li' + (r.options.collapse ? ' class="collapse in"' : "") + '><div class="datepicker">' + B.template + '</div></li><li class="picker-switch accordion-toggle"><a class="btn" style="width:100%"><span class="' + r.options.icons.time + '"></span></a></li><li' + (r.options.collapse ? ' class="collapse"' : "") + '><div class="timepicker">' + E.getTemplate() + "</div></li></ul>", e += "</div>"
            } return r.options.pickTime ? '<div class="bootstrap-datetimepicker-widget dropdown-menu"><div class="timepicker">' + E.getTemplate() + "</div></div>" : '<div class="bootstrap-datetimepicker-widget dropdown-menu"><div class="datepicker">' + B.template + "</div></div>"
        }, B = { modes: [{ clsName: "days", navFnc: "month", navStep: 1 }, { clsName: "months", navFnc: "year", navStep: 1 }, { clsName: "years", navFnc: "year", navStep: 10 }], headTemplate: '<thead><tr><th class="prev">&lsaquo;</th><th colspan="5" class="switch"></th><th class="next">&rsaquo;</th></tr></thead>', contTemplate: '<tbody><tr><td colspan="7"></td></tr></tbody>' }, E = { hourTemplate: '<span data-action="showHours"   data-time-component="hours"   class="timepicker-hour"></span>', minuteTemplate: '<span data-action="showMinutes" data-time-component="minutes" class="timepicker-minute"></span>', secondTemplate: '<span data-action="showSeconds"  data-time-component="seconds" class="timepicker-second"></span>' }
        B.template = '<div class="datepicker-days"><table class="table-condensed">' + B.headTemplate + '<tbody></tbody></table></div><div class="datepicker-months"><table class="table-condensed">' + B.headTemplate + B.contTemplate + '</table></div><div class="datepicker-years"><table class="table-condensed">' + B.headTemplate + B.contTemplate + "</table></div>", E.getTemplate = function () { return '<div class="timepicker-picker"><table class="table-condensed"><tr><td><a href="#" class="btn" data-action="incrementHours"><span class="' + r.options.icons.up + '"></span></a></td><td class="separator"></td><td>' + (r.options.useMinutes ? '<a href="#" class="btn" data-action="incrementMinutes"><span class="' + r.options.icons.up + '"></span></a>' : "") + "</td>" + (r.options.useSeconds ? '<td class="separator"></td><td><a href="#" class="btn" data-action="incrementSeconds"><span class="' + r.options.icons.up + '"></span></a></td>' : "") + (r.use24hours ? "" : '<td class="separator"></td>') + "</tr><tr><td>" + E.hourTemplate + '</td> <td class="separator">:</td><td>' + (r.options.useMinutes ? E.minuteTemplate : '<span class="timepicker-minute">00</span>') + "</td> " + (r.options.useSeconds ? '<td class="separator">:</td><td>' + E.secondTemplate + "</td>" : "") + (r.use24hours ? "" : '<td class="separator"></td><td><button type="button" class="btn btn-primary" data-action="togglePeriod"></button></td>') + '</tr><tr><td><a href="#" class="btn" data-action="decrementHours"><span class="' + r.options.icons.down + '"></span></a></td><td class="separator"></td><td>' + (r.options.useMinutes ? '<a href="#" class="btn" data-action="decrementMinutes"><span class="' + r.options.icons.down + '"></span></a>' : "") + "</td>" + (r.options.useSeconds ? '<td class="separator"></td><td><a href="#" class="btn" data-action="decrementSeconds"><span class="' + r.options.icons.down + '"></span></a></td>' : "") + (r.use24hours ? "" : '<td class="separator"></td>') + '</tr></table></div><div class="timepicker-hours" data-action="selectHour"><table class="table-condensed"></table></div><div class="timepicker-minutes" data-action="selectMinute"><table class="table-condensed"></table></div>' + (r.options.useSeconds ? '<div class="timepicker-seconds" data-action="selectSecond"><table class="table-condensed"></table></div>' : "") }, r.destroy = function () { I(), H(), r.widget.remove(), r.element.removeData("DateTimePicker"), r.component && r.component.removeData("DateTimePicker") }, r.show = function (e) {
            if (r.options.useCurrent && "" == p().val()) if (1 !== r.options.minuteStepping) {
                var t = o(), i = r.options.minuteStepping
                t.minutes(Math.round(t.minutes() / i) * i % 60).seconds(0), r.setValue(t.format(r.format))
            } else r.setValue(o().format(r.format))
            r.widget.show(), r.height = r.component ? r.component.outerHeight() : r.element.outerHeight(), m(), r.element.trigger({ type: "dp.show", date: o(r.date) }), q(), e && T(e)
        }, r.disable = function () {
            var e = r.element.find("input")
            e.prop("disabled") || (e.prop("disabled", !0), I())
        }, r.enable = function () {
            var e = r.element.find("input")
            e.prop("disabled") && (e.prop("disabled", !1), V())
        }, r.hide = function (t) {
            if (!t || !e(t.target).is(r.element.attr("id"))) {
                var i, a, n = r.widget.find(".collapse")
                for (i = 0; i < n.length; i++) if (a = n.eq(i).data("collapse"), a && a.date - transitioning) return
                r.widget.hide(), r.viewMode = r.startViewMode, P(), r.element.trigger({ type: "dp.hide", date: o(r.date) }), H()
            }
        }, r.setValue = function (e) { o.lang(r.options.language), e ? r.unset = !1 : (r.unset = !0, O()), o.isMoment(e) || (e = o(e, r.format)), e.isValid() ? (r.date = e, O(), r.viewDate = o({ y: r.date.year(), M: r.date.month() }), v(), D()) : f(e) }, r.getDate = function () { return r.unset ? null : r.date }, r.setDate = function (e) {
            var t = o(r.date)
            r.setValue(e ? e : null), u(t, "function")
        }, r.setDisabledDates = function (e) { r.options.disabledDates = j(e), r.viewDate && h() }, r.setEnabledDates = function (e) { r.options.enabledDates = j(e), r.viewDate && h() }, r.setMaxDate = function (e) { void 0 != e && (r.options.maxDate = o(e), r.viewDate && h()) }, r.setMinDate = function (e) { void 0 != e && (r.options.minDate = o(e), r.viewDate && h()) }, c()
    }
    e.fn.datetimepicker = function (t) {
        return this.each(function () {
            var i = e(this), a = i.data("DateTimePicker")
            a || i.data("DateTimePicker", new n(this, t))
        })
    }
})